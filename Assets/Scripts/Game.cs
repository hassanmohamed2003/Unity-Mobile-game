using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    private int frameCounter = 0;
    private Queue<BuildingPiece> nextBuildingPieces;
    public float newBlockStartY;
    
    [Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject rectanglePrefab;
    private List<GameObject> prefabList;
    private List<Color> colorList;

    public CameraFollow cameraScript;

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
            BuildingPiece piece = nextBuildingPieces.Dequeue();
            GameObject object1 = Instantiate(piece.prefab, position, rotation);
            newBlock = object1;
            SpriteRenderer renderer = object1.GetComponentInChildren<SpriteRenderer>();
            renderer.color = piece.color;
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
                Vector3 sizeBlock = collision.gameObject.transform.localScale;


                if (screenPos.y > 0.5f && screenPos.y < 1)
                    {
                    cameraScript.smoothMove = 0.1f;
                    counter++;
   
                    Vector3 viewportY = new Vector3(0, screenPos.y - 0.5f, 0);
                    Vector3 worldY = Camera.main.ViewportToWorldPoint(viewportY);

                    cameraScript.targetPos.y -= worldY.y;
                }
                if(screenPos.y > 1) {
                    cameraScript.targetPos.y = highestBlock.position.y;
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
        frameCounter++;
        if(frameCounter == 600){
            {
                Vector2 newBlockStartPosition = new(((float)Random.Range(-200, 200))/100.0f, newBlockStartY);
                SpawnNextPieceEndless(newBlockStartPosition, Quaternion.identity);
                frameCounter = 0;
            }
        }
    }
}
