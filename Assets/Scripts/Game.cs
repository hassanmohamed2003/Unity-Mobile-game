using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Game : MonoBehaviour
{
    public static Game instance;
    private Queue<GameObject> nextBuildingPieces;

    [Header("Particle System")]
    public GameObject particleBlocks;
    public GameObject particleScore;
    public GameObject particlePerfect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buildingHitSound;
    public AudioClip explosionSound;
    public AudioClip ropeBlinkSound;
    public AudioClip ropeBreakSound;
    public AudioClip perfectSound;
    public AudioClip levelCompleteSound;

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;
    public List<TextAsset> AvailableLevelFiles;
    public List<GameObject> AvailableBackgrounds;
    public List<PhysicsMaterial2D> AvailableBuildingPhysicsMaterials;
    public Transform blocksParent;

    [Header("Camera")]
    public CameraFollow cameraScript;
    public float CameraTargetHeight;

    [Header("Crane")]
    public Crane crane;

    [Header("UI")]
    public GameOverScreen EndlessGameOverScreen;
    public GameOverScreen LevelGameOverScreen;
    public TMP_Text TiltTutorial;
    public TMP_Text TapTutorial;
    public TMP_Text RopeTutorial;
    public TMP_Text EndScore;
    public TMP_Text EndStars;
    public TMP_Text currentScore;
    public TMP_Text highScore;
    public Animator animator;

    [Header("Game Behaviour")]
    public int CheckPoint;
    
    private List<GameObject> blocks = new();
    private GameObject firstBlock;
    private Transform highestBlock;
    private int counter = 0;
    private bool MoveCamera = false;
    private int spawnedBlockCounter = 0;
    private readonly string highScoreKey = "HighScore";
    private int highScoreAmount = 0;
    private bool isGameOver = false;
    private bool hasCompletedFirstPlay;
    private int firstStarRequirement;
    private int secondStarRequirement;
    private int thirdStarRequirement;

    void Awake()
    {
        if (instance == null)
            instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide Score for tutorial texts
        hasCompletedFirstPlay = PlayerPrefs.GetInt("HasCompletedFirstPlay", 0) == 1;
        if(!hasCompletedFirstPlay)
        {
            currentScore.gameObject.SetActive(false);
            crane.EnableRopeBreak = false;
        }

        if(GameState.IsEndless) StartEndless();
        else StartLevel();
    }

    private void StartEndless()
    {
        // Set default background
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[1].SetActive(true);

        CreatePieceQueue(Enumerable.Empty<GameObject>());
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        highScoreAmount = PlayerPrefs.GetInt(highScoreKey, 0);
    }

    private void StartLevel()
    {
        // Get the level structure from the level file
        LevelStructure structure = LevelStructure.GetLevelStructureFromAsset(AvailableLevelFiles[GameState.CurrentLevelID]);
            
        // Set the star requirements
        firstStarRequirement = structure.FirstStarScoreRequirement;
        secondStarRequirement = structure.SecondStarScoreRequirement;
        thirdStarRequirement = structure.ThirdStarScoreRequirement;

        // Transform piece IDs into actual pieces
        List<GameObject> pieces = structure.LevelPieceIDs.Select(id => AvailablePrefabs[id]).ToList();

        // Set the physics material for each building piece
        pieces.ForEach(piece => {
            if(piece.TryGetComponent(out Rigidbody2D rb))
            {
                rb.sharedMaterial = AvailableBuildingPhysicsMaterials[structure.BuildingPhysicsMaterialID];
            } 
        });

        // Build the queue with building pieces
        CreatePieceQueue(structure.LevelPieceIDs.Select(id => AvailablePrefabs[id]));

        // Turn all backgrounds off, only enable correct one
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[structure.BackgroundID].SetActive(true);
    }

    void CreatePieceQueue(IEnumerable<GameObject> pieces) {
        nextBuildingPieces = new Queue<GameObject>(pieces);
    }

    void AddRandomPieceToQueue() {
        nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
    }

    void SpawnNextPiece(Vector2 position, Quaternion rotation, bool endless) {
        if (nextBuildingPieces.Count > 0) {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Tutorial stuff
            spawnedBlockCounter++;
            if(!hasCompletedFirstPlay && spawnedBlockCounter == 1)
            {
                StartCoroutine(ShowTiltTutorial());
            }
            else if(!hasCompletedFirstPlay && spawnedBlockCounter == 3)
            {
                crane.EnableRopeBreak = true;
                StartCoroutine(ShowRopeTutorial());
            }

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, blocksParent);
            crane.SetConnectedPiece(gameObject);

            // Add a new random piece to the back of the queue if in endless mode
            if(endless) AddRandomPieceToQueue();
        }
        else
        {
            LevelGameOver();
        }

    }

    IEnumerator ShowTiltTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        TiltTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        TiltTutorial.gameObject.SetActive(false);

        StartCoroutine(ShowTapTutorial());
    }

    IEnumerator ShowTapTutorial()
    {
        yield return new WaitForSeconds(5.0f);
        TapTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        TapTutorial.gameObject.SetActive(false); 
    }

    IEnumerator ShowRopeTutorial()
    {
        yield return new WaitForSeconds(3.0f);
        RopeTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        RopeTutorial.gameObject.SetActive(false);
        currentScore.gameObject.SetActive(true);
        PlayerPrefs.SetInt("HasCompletedFirstPlay", 1);
        hasCompletedFirstPlay = true;
        PlayerPrefs.Save();
    }

    private void LevelGameOver()
    {
        isGameOver = true;
        crane.isGameOver = isGameOver;
        currentScore.text = "";
        EndScore.text = $"{counter}";
        int stars;
        if(counter >= thirdStarRequirement) stars = 3;
        else if(counter >= secondStarRequirement) stars = 2;
        else if(counter >= firstStarRequirement) stars = 1;
        else stars = 0;
        EndStars.text = $"{stars}";
        audioSource.PlayOneShot(levelCompleteSound);
        animator.SetTrigger("onGameOver");
        LevelGameOverScreen.Setup();
    }

    private void EndlessGameOver()
    {
        isGameOver = true;
        crane.isGameOver = isGameOver;
        currentScore.text = "";
        EndScore.text = $"{counter}";
        audioSource.PlayOneShot(explosionSound);
        OnGameOver(counter);
        animator.SetTrigger("onGameOver");
        EndlessGameOverScreen.Setup();
    }
    
    private void FreezeCheckpointBlock()
    {
        int index = blocks.Count - CheckPoint - 1;
        if(index >= 0 && blocks[index].TryGetComponent(out Rigidbody2D rb))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void checkPlacement(Collision2D collision)
    {
        if (collision.rigidbody)
        {
            float landingBock = collision.otherRigidbody.transform.position.x;
            float landedBock = collision.rigidbody.transform.position.x;
            if (landedBock - landingBock > 0.11 || landedBock - landingBock < -0.11)
            {
                Instantiate(particleScore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
            }
            else
            {
                audioSource.PlayOneShot(perfectSound);
                Instantiate(particlePerfect, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
                counter++;
            }
        }
        else
        {
            Instantiate(particleScore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
        }
    }

    public void HasDropped(Collision2D collision)
    {
        if (isGameOver)
        {
            return;
        }

        if (!blocks.Contains(collision.otherRigidbody.gameObject))
        {
            counter++;
            ContactPoint2D contact = collision.contacts[0];
            if(contact.normalImpulse > 100)
            {
                audioSource.PlayOneShot(buildingHitSound);
                Instantiate(particleBlocks, contact.point, Quaternion.identity);
            }
            blocks.Add(collision.otherRigidbody.gameObject);
            FreezeCheckpointBlock();
            checkPlacement(collision);
            currentScore.text = $"{counter}";
        }

        if (collision.gameObject.CompareTag("Landed Block"))
        {
            if (highestBlock != null)
            {
                MoveCamera = true;
            }



            if (highestBlock == null || collision.gameObject.transform.position.y > highestBlock.position.y)
            {

                highestBlock = collision.gameObject.transform;
            }
        }

        if (firstBlock == null)
        {

            firstBlock = collision.otherRigidbody.gameObject;
        }
        else if (collision.gameObject.TryGetComponent(out Floor floor) && collision.otherRigidbody.gameObject != firstBlock)
        {
            blocks.Remove(collision.otherRigidbody.gameObject);
            crane.transform.position.Set(0.0f, crane.transform.position.y, crane.transform.position.z);
            if(GameState.IsEndless) EndlessGameOver();
            else LevelGameOver();
        }
    }


    void LateUpdate()
    {
        if (MoveCamera)
        {
            // Get the camera's world position at the specified target height
            Vector3 cameraWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, CameraTargetHeight, 0));

            // Check if the highest block is above the target height
            if( highestBlock.gameObject.TryGetComponent(out BuildingBlock block) && 
                (block.transform.position.y + block.ropeStartOffset) > cameraWorldPos.y)
            {
                // Calculate offset from bottom to highest block
                Vector3 bottomScreenWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
                float offset = block.transform.position.y + block.ropeStartOffset - bottomScreenWorldPos.y;

                // Add multiple of offset to camera's targetpos to update it
                cameraScript.targetPos.y = bottomScreenWorldPos.y + (0.5f/CameraTargetHeight * offset);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (crane.IsReadyForNextPiece && !isGameOver){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            SpawnNextPiece(newBlockPos, Quaternion.identity, GameState.IsEndless);
        }
    }

    private void OnGameOver(int newScore){
        if (newScore > highScoreAmount)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
        }
        currentScore.text = "";
        highScore.text = $"{PlayerPrefs.GetInt("HighScore", highScoreAmount)}";
    }
}
