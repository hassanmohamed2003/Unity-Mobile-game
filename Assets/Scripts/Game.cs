using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms.Impl;

public class Game : MonoBehaviour
{
    public static Game instance;
    private Queue<GameObject> nextBuildingPieces;

    [Header("Particle System")]
    public GameObject particleBlocks;
    public GameObject particleScore;
    public GameObject particlePerfect;
    public GameObject particleHighscore;
    public GameObject arthurHappy;
    public GameObject arthurMad;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buildingHitSound;
    public AudioClip explosionSound;
    public AudioClip ropeBlinkSound;
    public AudioClip ropeBreakSound;
    public AudioClip perfectSound;
    public List<AudioClip> comboSounds;
    public AudioClip levelCompleteSound;

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;
    public List<GameObject> AvailableArthurs;
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
    public Transform blocksParent;
    public Transform canvas;

    [Header("Game Behaviour")]
    public int CheckPoint;
    
    private List<GameObject> blocks = new();
    private GameObject firstBlock;
    private Transform highestBlock;
    private int counter = 0;
    private int comboCounter = 0;
    private float lastCloudSpawned;
    bool MoveCamera = false;
    private int spawnedBlockCounter = 0;
    private readonly string highScoreKey = "HighScore";
    private int highScoreAmount = 0;
    private bool isGameOver = false;
    private bool hasCompletedFirstPlay;
    private bool hasHighscore = false;
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

    void AddRandomPieceToQueue() 
    {
        if(nextBuildingPieces.Count == 0)
        {
            nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
            return;
        }
        GameObject newPrefab = nextBuildingPieces.Last();
        while(newPrefab == nextBuildingPieces.Last())
        {
            newPrefab = AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)];
        }
        nextBuildingPieces.Enqueue(newPrefab);
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
        yield return new WaitForSeconds(3.0f);
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
        UpdateCheckpoint();
    }

    private void UpdateCheckpoint()
    {
        const int highScoreThreshold = 210;
        if(blocks.Count < highScoreThreshold)
        {
            CheckPoint = 4 + blocks.Count/15;
        }
        else if(blocks.Count > highScoreThreshold)
        {
            int lowerScore = 4 + highScoreThreshold/15;
            CheckPoint = lowerScore + (blocks.Count - highScoreThreshold)/100;
        }
    }

    private void comboCheck()
    {
        Debug.Log(comboCounter);
        if (comboCounter < 1)
        {
            AvailableArthurs[0].SetActive(false);
            audioSource.PlayOneShot(comboSounds[comboCounter]);
            comboCounter++;
        }

        else if(comboCounter == 1)
        {
            audioSource.PlayOneShot(comboSounds[comboCounter]);

            AvailableArthurs[0].SetActive(true);

            comboCounter = 0;
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
                arthurHappy.SetActive(false);
                comboCounter = 0;
                Instantiate(particleScore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
            }
            else
            {
                Instantiate(particlePerfect, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
                counter++;
                comboCheck();
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
            collision.otherRigidbody.velocity = Vector3.zero;
            collision.otherRigidbody.angularVelocity = 0;
            collision.otherRigidbody.constraints = RigidbodyConstraints2D.None;
            collision.otherRigidbody.freezeRotation = false;
            crane.HasLastPieceLanded = true;
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
            //Change Arthurs position
            if (AvailableArthurs[0].activeSelf)
            {
                AvailableArthurs[0].transform.position = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.1f));
                Vector3 arthurHappy = AvailableArthurs[0].transform.position;

                AvailableArthurs[0].transform.position = new Vector3(arthurHappy.x, arthurHappy.y, 0);
            }

            if (AvailableArthurs[1].activeSelf)
            {
                AvailableArthurs[1].transform.position = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.1f));


                Vector3 arthurMad = AvailableArthurs[1].transform.position;

                AvailableArthurs[1].transform.position = new Vector3(arthurMad.x, arthurMad.y, 0);

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

    public void launchConfetti()
    {
        if (hasHighscore)
        {
            Instantiate(particleHighscore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), Quaternion.identity);
        }
    }

    private void OnGameOver(int newScore){
        if (newScore > highScoreAmount)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
            hasHighscore = true;
        }
        else
        {
            AvailableArthurs[1].SetActive(true);
        }
        currentScore.text = "";
        highScore.text = $"{PlayerPrefs.GetInt("HighScore", highScoreAmount)}";
    }
}
