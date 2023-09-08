using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    public TMP_Text currentScore;
    public TMP_Text highScore;

    public GameOverScreen GameOverScreen;
    private List<GameObject> blocks = new List<GameObject>();
    private GameObject firstBlock;

    private int counter = 0;

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
    }

    void CreatePieceQueue(IEnumerable<GameObject> pieces){
        nextBuildingPieces = new Queue<GameObject>(pieces);
    }

    void AddPieceToQueue(GameObject prefab){
        nextBuildingPieces.Enqueue(prefab);
    }

    void AddRandomPieceToQueue(){
        nextBuildingPieces.Enqueue(AvailablePrefabs[UnityEngine.Random.Range(0, AvailablePrefabs.Count)]);
    }

    void EmptyPieceQueue(){
        nextBuildingPieces.Clear();
    }
    void SpawnNextPieceEndless(Vector2 position, Quaternion rotation){
        if(nextBuildingPieces.Count > 0){
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, parent);
            crane.SetConnectedPiece(gameObject);

            // Add a new random piece to the back of the queue
            AddRandomPieceToQueue();
        }

    }

    private void GameOver()
    {
        Debug.Log(blocks.Count);
        currentScore.text = $"Score: {blocks.Count}";
        highScore.text = $"High Score: {PlayerPrefs.GetInt("highscore", 0)}";

        OnGameOver(blocks.Count);
        Debug.Log(blocks.Count);
        GameOverScreen.Setup();
        Time.timeScale = 0;
    }

    public void HasDropped(Collision2D collision)
    {
        counter++;
        if (!blocks.Contains(collision.otherRigidbody.gameObject))
        {
            blocks.Add(collision.otherRigidbody.gameObject);
        }

        if (collision.gameObject.CompareTag("Landed Block"))
        {
            if (highestBlock != null)
            {
                Vector3 screenPos = Camera.main.WorldToViewportPoint(highestBlock.position);
                Vector3 blockPos = highestBlock.position;
                Vector3 cameraY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));


                if (blockPos.y > cameraY.y)
                    {
                    cameraScript.smoothMove = 1;

                    cameraScript.targetPos.y += blockPos.y - cameraY.y;
                }

                if (screenPos.y < 0.5f)
                {
                    cameraScript.smoothMove = 0;
                }

            }



                if (highestBlock == null || collision.gameObject.transform.position.y > highestBlock.position.y)
                {

                    highestBlock = collision.gameObject.transform;
            }
        }
        if(firstBlock == null)
        {

            firstBlock = collision.otherRigidbody.gameObject;
        }
        else if(collision.gameObject.CompareTag("Floor") && collision.otherRigidbody.gameObject != firstBlock)
        {
            GameOver();
        }
        Debug.Log(collision.otherRigidbody.gameObject != firstBlock);

    }



    // Update is called once per frame
    void Update()
    {
        if(crane.IsReadyForNextPiece){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            SpawnNextPieceEndless(newBlockPos, Quaternion.identity);
        }
    }

    private void OnGameOver(int newScore){
        if(PlayerPrefs.HasKey("highscore")){
            int currentHighScore = PlayerPrefs.GetInt("highscore", int.MaxValue);
            if(newScore > currentHighScore){
                PlayerPrefs.SetInt("highscore", newScore);
            }
        }
    }
}
