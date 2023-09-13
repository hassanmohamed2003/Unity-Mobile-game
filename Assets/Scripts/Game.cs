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

    [Header("Prefabs")]
    public List<GameObject> AvailablePrefabs;
    public Transform parent;
    private List<Color> colorList;

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
    private HashSet<GameObject> uniqueBlocks = new HashSet<GameObject>();
    private GameObject firstBlock;

    private int counter = 0;
    bool MoveCamera = false;
    bool PieceDropped = false;
    public float moveAmount;

    string highScoreKey = "HighScore";
    public int scoreAmount = 0;
    public int highScoreAmount = 0;

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
            GameObject gameObject = Instantiate(prefab, position, rotation, parent);
            newBlock = gameObject;
            uniqueBlocks.Add(newBlock);
            crane.SetConnectedPiece(gameObject);
            PieceDropped = true;

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
    public void HasDropped(Collision2D collision)
    {
        PieceDropped = true;
        counter++;

        if (!blocks.Contains(collision.otherRigidbody.gameObject))
        {
            blocks.Add(collision.otherRigidbody.gameObject);
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
            GameOver();
        }
    }


    void LateUpdate()
    {
        if (MoveCamera)
        {
            Vector3 screenPos = Camera.main.WorldToViewportPoint(highestBlock.position);
            Vector3 blockPos = highestBlock.position;
            Vector3 cameraY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

            Debug.Log(blockPos);
            Debug.Log(cameraY);

            if (blockPos.y > cameraY.y && crane.released)
            {
                cameraScript.smoothMove = moveAmount;
                cameraScript.targetPos.y += blockPos.y - cameraY.y;
                PieceDropped = false;
            }
            else
            {
                cameraScript.smoothMove = 0;
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
