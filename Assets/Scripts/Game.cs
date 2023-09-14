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

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public List<GameObject> AvailableClouds;
    public Transform blocksParent;
    public Transform cloudsParent;
    private List<Color> colorList;
    public int CheckPoint;

    public CameraFollow cameraScript;
    public Crane crane;
    private Transform highestBlock;
    private GameObject newBlock;
    private Transform middleScreen;
    public TMP_Text EndScore;
    public TMP_Text currentScore;
    public TMP_Text highScore;

    public GameOverScreen GameOverScreen;
    private List<GameObject> blocks = new List<GameObject>();
    private GameObject firstBlock;

    private int counter = 0;
    private float lastCloudSpawned;
    public float cloudSpawnInterval;
    bool MoveCamera = false;
    bool PieceDropped = false;

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
        colorList = new(){
            Color.red,
            Color.black,
            Color.blue,
            Color.green,
            Color.white
        };
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

    void AddPieceToQueue(GameObject prefab) {
        nextBuildingPieces.Enqueue(prefab);
    }

    void AddRandomPieceToQueue() {
        nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
    }

    void EmptyPieceQueue() {
        nextBuildingPieces.Clear();
    }
    void SpawnNextPieceEndless(Vector2 position, Quaternion rotation) {
        if (nextBuildingPieces.Count > 0) {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, blocksParent);
            crane.SetConnectedPiece(gameObject);

            newBlock = gameObject;

            // Add a new random piece to the back of the queue
            AddRandomPieceToQueue();
        }

    }

    private void GameOver()
    {
        currentScore.text = "";
        Debug.Log(blocks.Count);
        EndScore.text = $"Score: {blocks.Count}";
        OnGameOver(blocks.Count);
        Debug.Log(blocks.Count);
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
        counter++;
        PieceDropped = true;
        if (!blocks.Contains(collision.otherRigidbody.gameObject))
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
            SpawnNextPieceEndless(newBlockPos, Quaternion.identity);
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
