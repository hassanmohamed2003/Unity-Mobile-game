using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;

public class Game : MonoBehaviour
{
    private int frameCounter = 0;
    private Queue<BuildingPiece> nextBuildingPieces;
    public float newBlockStartY;
    
    [Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject rectanglePrefab;

    private struct BuildingPiece{
        public GameObject prefab; // Use Resources.Load() to find the prefab
        public Color color; // Color of the block
    }

    // Start is called before the first frame update
    void Start()
    {
        List<BuildingPiece> pieceList = new();
        BuildingPiece piece1 = new()
        {
            prefab = blockPrefab,
            color = Color.white
        };
        BuildingPiece piece2 = new(){
            prefab = rectanglePrefab,
            color = Color.cyan
        };
        BuildingPiece piece3 = new(){
            prefab = rectanglePrefab,
            color = Color.green
        };
        BuildingPiece piece4 = new(){
            prefab = blockPrefab,
            color = Color.red
        };
        pieceList.Add(piece1);
        pieceList.Add(piece2);
        pieceList.Add(piece3);
        pieceList.Add(piece4);
        CreatePieceQueue(pieceList);
    }

    void CreatePieceQueue(IEnumerable<BuildingPiece> pieces){
        nextBuildingPieces = new Queue<BuildingPiece>(pieces);
    }

    void SpawnNextPiece(Vector2 position, Quaternion rotation){
        if(nextBuildingPieces.Count > 0){
            BuildingPiece piece = nextBuildingPieces.Dequeue();
            GameObject object1 = Instantiate(piece.prefab, position, rotation);
            SpriteRenderer renderer = object1.GetComponentInChildren<SpriteRenderer>();
            renderer.color = piece.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;
        if(frameCounter == 600){
            Vector2 newBlockStartPosition = new Vector2(Random.Range(-2, 2), newBlockStartY);
            SpawnNextPiece(newBlockStartPosition, Quaternion.identity);           
            frameCounter = 0;
        }
    }
}
