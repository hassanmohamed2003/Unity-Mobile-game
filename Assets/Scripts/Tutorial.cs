using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    private Queue<GameObject> nextBuildingPieces;

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;
    public List<Sprite> AvailableBackgrounds;
    public Transform blocksParent;
    public Transform cloudsParent;
    public int CheckPoint;

    public CameraFollow cameraScript;
    public float cameraTargetHeight;
    public Crane crane;
    private Transform highestBlock;
    public TMP_Text EndScore;
    public TMP_Text currentScore;
    public TMP_Text highScore;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buildingHitSound;
    public AudioClip explosionSound;
    public AudioClip ropeBlinkSound;
    public AudioClip ropeBreakSound;

    public GameOverScreen GameOverScreen;
    public GameOverScreen TiltTutorial;
    public GameOverScreen TapTutorial;
    public GameOverScreen RopeTutorial;
    private readonly List<GameObject> blocks = new();
    private GameObject firstBlock;
    public float cloudSpawnInterval;
    bool MoveCamera = false;

    string highScoreKey = "HighScore";
    public int scoreAmount = 0;
    public int highScoreAmount = 0;
    public float moveAmount;
    public int spawnedBlockCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the high score
        highScoreAmount = PlayerPrefs.GetInt(highScoreKey, 0);

        // Setup tutorial
        StartTutorial();
    }

    void StartTutorial()
    {   
        // Get the id of the level
        int levelID = LevelSelector.CurrentLevelID;

        // Convert to LevelStructure object
        LevelStructure level = LevelSelector.Levels[levelID].GetComponent<LevelStructure>();

        // Convert pieces from IDs to actual prefabs
        CreatePieceQueue(level.pieces);
    }

    void CreatePieceQueue(IEnumerable<GameObject> pieces) {
        nextBuildingPieces = new Queue<GameObject>(pieces);
    }

    void AddRandomPieceToQueue() {
        nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
    }

    void SpawnNextPiece(Vector2 position, Quaternion rotation, bool endless) {
        Debug.Log(nextBuildingPieces.Count);
        if (nextBuildingPieces.Count > 0) {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, blocksParent);
            crane.SetConnectedPiece(gameObject);

            spawnedBlockCount++;
            if(spawnedBlockCount == 1)
            {
                IEnumerator tiltTutorial = ShowTiltTutorial();
                IEnumerator tapTutorial = ShowTapTutorial();
                StartCoroutine(tiltTutorial);
                StartCoroutine(tapTutorial);
            }
            else if(spawnedBlockCount == 2)
            {
                IEnumerator ropeTutorial = ShowRopeTutorial();
                StartCoroutine(ropeTutorial);
            }

            // If in endless mode add a new piece to the queue
            if(endless) AddRandomPieceToQueue();
        }
        else
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        GameOverScreen.Setup();
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        currentScore.text = "";
        EndScore.text = $"Please retry the tutorial";
        GameOverScreen.Setup();
        Time.timeScale = 0;
    }

    IEnumerator ShowTiltTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        TiltTutorial.Setup();
        yield return new WaitForSeconds(4.0f);
        TiltTutorial.gameObject.SetActive(false);
    }

    IEnumerator ShowTapTutorial()
    {
        yield return new WaitForSeconds(10.0f);
        TapTutorial.Setup();
        yield return new WaitForSeconds(3.0f);
        TapTutorial.gameObject.SetActive(false);
    }

    IEnumerator ShowRopeTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        RopeTutorial.Setup();
        yield return new WaitForSeconds(5.0f);
        RopeTutorial.gameObject.SetActive(false);
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

    public void HasDropped(Collision2D collision)
    {
        audioSource.PlayOneShot(buildingHitSound);
        if (!blocks.Contains(collision.otherRigidbody.gameObject) && !collision.gameObject.CompareTag("Wall"))
        {
            blocks.Add(collision.otherRigidbody.gameObject);
            FreezeCheckpointBlock();
            currentScore.text = $"{blocks.Count}";
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
            audioSource.PlayOneShot(explosionSound);
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
            Vector3 cameraWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, cameraTargetHeight, 0));

            // Check if the highest block is above the target height
            if( highestBlock.gameObject.TryGetComponent(out BuildingBlock block) && 
                (block.transform.position.y + block.ropeStartOffset) > cameraWorldPos.y)
            {
                // Calculate offset from bottom to highest block
                Vector3 bottomScreenWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
                float offset = block.transform.position.y + block.ropeStartOffset - bottomScreenWorldPos.y;

                // Add multiple of offset to camera's targetpos to update it
                cameraScript.targetPos.y = bottomScreenWorldPos.y + (0.5f/cameraTargetHeight * offset);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (crane.IsReadyForNextPiece){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            SpawnNextPiece(newBlockPos, Quaternion.identity, LevelSelector.IsEndless);            
        }
    }
}
