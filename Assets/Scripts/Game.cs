using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MPE;
using UnityEngine;

public class Game : MonoBehaviour
{
    private int frameCounter = 0;
    public BuildingBlock blockPrefab;
    public BuildingBlockLong blockLongPrefab;
    private Queue<BuildingPieceType> nextBuildingPieces;

    public float newBlockStartY;

    enum BuildingPieceType{
        SQUARE,
        RECTANGLE
    };

    // Start is called before the first frame update
    void Start()
    {
        List<BuildingPieceType> list = new()
        {
            BuildingPieceType.SQUARE,
            BuildingPieceType.RECTANGLE,
            BuildingPieceType.SQUARE,
            BuildingPieceType.SQUARE,
            BuildingPieceType.RECTANGLE,
            BuildingPieceType.RECTANGLE
        };
        createPieceQueue(list);
    }

    void createPieceQueue(IEnumerable<BuildingPieceType> pieces){
        nextBuildingPieces = new Queue<BuildingPieceType>(pieces);
    }

    void spawnNextPiece(Vector2 position, Quaternion quaternion){
        if(nextBuildingPieces.Count > 0){
            BuildingPieceType type = nextBuildingPieces.Dequeue();
            if(type == BuildingPieceType.SQUARE){
                Instantiate(blockPrefab, position, quaternion);
            }
            else if(type == BuildingPieceType.RECTANGLE){
                Instantiate(blockLongPrefab, position, quaternion);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;
        if(frameCounter == 600){
            Vector2 newBlockStartPosition = new Vector2(0, 4);
            spawnNextPiece(newBlockStartPosition, Quaternion.identity);           
            frameCounter = 0;
        }
    }
}
