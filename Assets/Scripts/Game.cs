using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// copyright by dreamberd
public class Game : MonoBehaviour
{
    public static Game instance;
    private Queue<GameObject> nextBuildingPieces;
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


    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;

    public CameraFollow cameraScript;
    public float CameraTargetHeight;
    public Crane crane;
    private Transform highestBlock;
    public TMP_Text EndScore;
    public TMP_Text currentScore;
    public TMP_Text highScore;
    public Animator animator;
    public Transform blocksParent;
    public Transform canvas;
    public int CheckPoint;

    public GameOverScreen GameOverScreen;
    public TMP_Text TiltTutorial;
    public TMP_Text TapTutorial;
    public TMP_Text RopeTutorial;
    private List<GameObject> blocks = new List<GameObject>();
    private GameObject firstBlock;

    private int counter = 0;
    private int comboCounter = 0;
    private float lastCloudSpawned;
    bool MoveCamera = false;
    private int spawnedBlockCounter = 0;

    private string highScoreKey = "HighScore";
    public int scoreAmount = 0;
    public int highScoreAmount = 0;
    public float moveAmount;
    public bool isGameOver = false;
    private bool hasCompletedFirstPlay;
    private bool hasHighscore = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.SetInt("HasCompletedFirstPlay", 0);
        // PlayerPrefs.Save();
        // Hide Score for tutorial texts
        hasCompletedFirstPlay = PlayerPrefs.GetInt("HasCompletedFirstPlay", 0) == 1;
        if(!hasCompletedFirstPlay)
        {
            currentScore.gameObject.SetActive(false);
            crane.EnableRopeBreak = false;
        }

        CreatePieceQueue(Enumerable.Empty<GameObject>());
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        highScoreAmount = PlayerPrefs.GetInt(highScoreKey, 0);
        lastCloudSpawned = Time.realtimeSinceStartup;
    }

    void CreatePieceQueue(IEnumerable<GameObject> pieces) {
        nextBuildingPieces = new Queue<GameObject>(pieces);
    }

    void AddRandomPieceToQueue() {
        nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
    }

    void SpawnNextPieceEndless(Vector2 position, Quaternion rotation) {
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

            // Add a new random piece to the back of the queue
            AddRandomPieceToQueue();
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

    private void GameOver()
    {
        arthurMad.SetActive(true);
        isGameOver = true;
        crane.isGameOver = isGameOver;
        currentScore.text = "";
        EndScore.text = $"{counter}";
        audioSource.PlayOneShot(explosionSound);
        OnGameOver(counter);
        animator.SetTrigger("onGameOver");
        GameOverScreen.Setup();
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

    private void comboCheck()
    {
        Debug.Log(comboCounter);

        if (comboCounter == 1)
        {
            Debug.Log("artur");
            arthurHappy.SetActive(true);
        }
        if (comboCounter < 3)
        {
            audioSource.PlayOneShot(comboSounds[comboCounter]);
            comboCounter++;
        }
        else if(comboCounter > 3)
        {
            audioSource.PlayOneShot(comboSounds.Last());
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
            GameOver();
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
        if (crane.IsReadyForNextPiece){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            SpawnNextPieceEndless(newBlockPos, Quaternion.identity);
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
            arthurMad.SetActive(true);
        }
        currentScore.text = "";
        highScore.text = $"{PlayerPrefs.GetInt("HighScore", highScoreAmount)}";
    }
}
