using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    private Queue<GameObject> nextBuildingPieces;

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;
    public List<Sprite> AvailableBackgrounds;
    public Transform blocksParent;
    public Transform cloudsParent;
    public int CheckPoint;

    public CameraFollow cameraScript;
    public Crane crane;
    private Transform highestBlock;
    public TMP_Text EndScore;
    public TMP_Text currentScore;
    public TMP_Text highScore;
    public AudioSource audioSource;
    public AudioClip buildingHitSound;
    public AudioClip explosionSound;

    public GameOverScreen GameOverScreen;
    private readonly List<GameObject> blocks = new();
    private GameObject firstBlock;
    public float cloudSpawnInterval;
    bool MoveCamera = false;

    string highScoreKey = "HighScore";
    public int scoreAmount = 0;
    public int highScoreAmount = 0;
    public float moveAmount;

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

        // Check if we're starting a level or playing endless mode
        Debug.Log($"Is Endless? {LevelSelector.IsEndless}");
        if(LevelSelector.IsEndless) StartEndless();
        else StartLevel();
    }

    void StartEndless()
    {
        // Create a Queue with random pieces
        CreatePieceQueue(Enumerable.Empty<GameObject>());
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
    }

    void StartLevel()
    {   
        // Get the id of the level
        int levelID = LevelSelector.CurrentLevelID;

        // Convert to LevelStructure object
        LevelStructure level = LevelSelector.Levels[levelID - 1].GetComponent<LevelStructure>();

        Debug.Log($"{level.levelName} loaded");

        Debug.Log($"Pieces to place: {level.pieces.Count}");

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
        if (nextBuildingPieces.Count > 0) {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, blocksParent);
            crane.SetConnectedPiece(gameObject);

            // If in endless mode add a new piece to the queue
            if(endless) AddRandomPieceToQueue();
        }
    }

    private void GameOver()
    {
        currentScore.text = "";
        EndScore.text = $"Score: {blocks.Count}";
        OnGameOver(blocks.Count);
        GameOverScreen.Setup();
        Time.timeScale = 0;
    }
    
    private void FreezeCheckpointBlock()
    {
        int index = blocks.Count - CheckPoint - 1;
        if(index >= 0 && blocks[index].TryGetComponent(out Rigidbody2D rb))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
            Vector3 cameraY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

            if( highestBlock.gameObject.TryGetComponent(out BuildingBlock block) && 
                (block.transform.position.y + block.ropeStartOffset) > cameraY.y)
            {
                cameraScript.targetPos.y = block.transform.position.y + block.ropeStartOffset;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        // if(Time.realtimeSinceStartup + lastCloudSpawned > cloudSpawnInterval)
        // {
        //     GameObject cloudPrefab = AvailableClouds[UnityEngine.Random.Range(0, AvailableClouds.Count)];
        //     Vector3 cloudPos = new(0.0f, 0.0f, 0.0f);
        //     Instantiate(cloudPrefab, cloudPos, Quaternion.identity, cloudsParent);
        // }
        if (crane.IsReadyForNextPiece){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            if(LevelSelector.IsEndless)
            {
                SpawnNextPiece(newBlockPos, Quaternion.identity, true);
            }
            else if(nextBuildingPieces.Count > 0)
            {
                SpawnNextPiece(newBlockPos, Quaternion.identity, false);
            }
            
        }
    }

    private void OnGameOver(int newScore){
        if (newScore > highScoreAmount)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
        }
        currentScore.text = "";
        highScore.text = $"High Score: {PlayerPrefs.GetInt("HighScore", highScoreAmount)}";
    }
}
