using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BlockSystem : MonoBehaviour
{
    [Header("Block Parent")]
    public Transform blocksParent;

    [Header("Prefabs")]
    public List<GameObject> availablePrefabs;
    public List<PhysicsMaterial2D> availableBuildingPhysicsMaterials;

    [Header("Events")]
    public UnityEvent<Transform> HighestBlockUpdatedEvent;
    public UnityEvent<Collision2D> NewBlockLandedEvent;
    public UnityEvent<GameObject> NewBlockSpawnedEvent;
    public UnityEvent OutOfBlocksEvent;

    [Header("Crane Transform")]
    public Transform CraneTransform;

    private Queue<GameObject> nextBuildingPieces = new();
    private List<GameObject> landedBlocks = new();
    private int spawnedBlockCounter = 0;
    private Transform highestBlock;
    public bool HasFirstBlockLanded {get; private set;} = false;

    public void CreatePieceQueueFromLevel(LevelStructure structure)
    {
        IEnumerable<int> pieceIDs = structure.LevelPieceIDs;
        int buildingPhysicsMaterialID = structure.BuildingPhysicsMaterialID;
        List<GameObject> prefabs = pieceIDs.Select(id => availablePrefabs[id]).ToList();
        
        prefabs.ForEach(piece => {
            if(piece.TryGetComponent(out Rigidbody2D rb))
            {
                rb.sharedMaterial = availableBuildingPhysicsMaterials[buildingPhysicsMaterialID];
            } 
        });
        nextBuildingPieces = new Queue<GameObject>(prefabs);
    }

    public void AddRandomPieceToQueue() 
    {
        if(nextBuildingPieces.Count == 0)
        {
            nextBuildingPieces.Enqueue(availablePrefabs[Random.Range(0, availablePrefabs.Count)]);
            return;
        }
        GameObject newPrefab = nextBuildingPieces.Last();
        while(newPrefab == nextBuildingPieces.Last())
        {
            newPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
        }
        nextBuildingPieces.Enqueue(newPrefab);
    }

    public void SpawnNextPiece() {
        if (nextBuildingPieces.Count > 0 && !Game.instance.IsGameOver) 
        {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Tutorial stuff
            spawnedBlockCounter++;
            Game.instance.OnPieceSpawnedFirstPlay(spawnedBlockCounter);

            if(prefab.TryGetComponent(out BuildingBlock block))
            {
                block.blockSystem = this;
            }

            Vector2 newBlockPos = CraneTransform.position;
            newBlockPos.y -= 2.0f;
            GameObject gameObject = Instantiate(prefab, newBlockPos, Quaternion.identity, blocksParent);

            // Let other systems know a new block has been spawned
            NewBlockSpawnedEvent.Invoke(gameObject);

            // Add a new random piece to the back of the queue if in endless mode
            if(GameState.IsEndless) AddRandomPieceToQueue();
        }
        else
        {
            // Let other systems know that the block system is out of blocks (Level completed)
            OutOfBlocksEvent.Invoke();
        }
    }

    public bool LandedBlocksContains(GameObject block)
    {
        return landedBlocks.Contains(block);
    }

    public void AddLandedBlock(GameObject block)
    {
        landedBlocks.Add(block);
    }

    public List<GameObject> GetLandedBlocks()
    {
        return landedBlocks;
    }

    public void OnBlockCollision(Collision2D collision)
    {
        // Check if this block hasn't already landed before
        if(!LandedBlocksContains(collision.otherRigidbody.gameObject))
        {
            // If it hasn't add it to the list
            AddLandedBlock(collision.otherRigidbody.gameObject);
            
            // Let other systems know a new block has landed
            NewBlockLandedEvent.Invoke(collision);

            SpawnNextPiece();

            HasFirstBlockLanded = true;
        }
        UpdateHighestBlockOnCollision(collision);
    }

    public void UpdateHighestBlockOnCollision(Collision2D collision)
    {
        // If this new colliding block is positioned higher then the current highest block
        if(highestBlock == null || collision.gameObject.transform.position.y > highestBlock.position.y)
        {
            // Set it as the new highest block
            highestBlock = collision.gameObject.transform;

            // Let other systems know there is a new highest block
            HighestBlockUpdatedEvent.Invoke(highestBlock);
        }
    }
}
