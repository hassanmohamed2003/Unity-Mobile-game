using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    private int moveCount = 0;
    private int frameCounter = 0;
    private Queue<BuildingPiece> nextBuildingPieces;
    public float distanceCraneToPieceOnSpawn;
    
    [Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject rectanglePrefab;
    public Transform parent;
    private List<GameObject> prefabList;
    private List<Color> colorList;

    public CameraFollow cameraScript;
    public Crane crane;

    private Transform highestBlock;
    private GameObject newBlock;
    private Transform middleScreen;

    float counter;

    private struct BuildingPiece{
        public GameObject prefab; // Use Resources.Load() to find the prefab
        public Color color; // Color of the block
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        prefabList = new(){
            blockPrefab,
            rectanglePrefab
        };

        colorList = new(){
            Color.red,
            Color.black,
            Color.blue,
            Color.green,
            Color.white
        };
        CreatePieceQueue(Enumerable.Empty<BuildingPiece>());
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
        AddRandomPieceToQueue();
    }

    void CreatePieceQueue(IEnumerable<BuildingPiece> pieces){
        nextBuildingPieces = new Queue<BuildingPiece>(pieces);
    }

    void AddPieceToQueue(BuildingPiece piece){
        nextBuildingPieces.Enqueue(piece);
    }

    void AddRandomPieceToQueue(){
        nextBuildingPieces.Enqueue(new BuildingPiece(){
            prefab = prefabList[Random.Range(0, prefabList.Count)],
            color = colorList[Random.Range(0, colorList.Count)]
        });
    }

    void EmptyPieceQueue(){
        nextBuildingPieces.Clear();
    }
    void SpawnNextPieceEndless(Vector2 position, Quaternion rotation){
        if(nextBuildingPieces.Count > 0){
            // Get the next building piece
            BuildingPiece piece = nextBuildingPieces.Dequeue();

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(piece.prefab, position, rotation, parent);
            crane.SetConnectedPiece(gameObject);
            newBlock = object1;

            // Give sprite correct color
            SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            renderer.color = piece.color;

            // Add a new random piece to the back of the queue
            AddRandomPieceToQueue();
        }

    }

    public void HasDropped(Collision2D collision)
    {
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
                    counter++;

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
}
