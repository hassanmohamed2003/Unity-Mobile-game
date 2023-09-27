using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    [Header("Block Parent")]
    public Transform blocksParent;

    [Header("Prefabs")]
    public List<GameObject> availablePrefabs;
    public List<PhysicsMaterial2D> availableBuildingPhysicsMaterials;

    [Header("Crane")]
    public Crane crane;

    private Queue<GameObject> nextBuildingPieces;
    private List<GameObject> landedBlocks = new();
    private int spawnedBlockCounter = 0;
    private GameObject firstBlock;
    public bool FirstBlockPlaced{ get; private set; } = false;

    public void CreatePieceQueue(IEnumerable<GameObject> pieces) {
        nextBuildingPieces = new Queue<GameObject>(pieces);
    }

    public void CreatePieceQueue(IEnumerable<int> pieceIDs, int buildingPhysicsMaterialID)
    {
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

    public void SpawnNextPiece(Vector2 position, Quaternion rotation, bool endless) {
        if (nextBuildingPieces.Count > 0) 
        {
            // Get the next building piece
            GameObject prefab = nextBuildingPieces.Dequeue();

            // Tutorial stuff
            spawnedBlockCounter++;
            Game.instance.OnPieceSpawnedFirstPlay(spawnedBlockCounter);

            // Instantiate the object and connect to crane
            GameObject gameObject = Instantiate(prefab, position, rotation, blocksParent);
            crane.SetConnectedPiece(gameObject);

            // Add a new random piece to the back of the queue if in endless mode
            if(endless) AddRandomPieceToQueue();
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

    public void SetFirstBlock(GameObject block)
    {
        FirstBlockPlaced = true;
        firstBlock = block;
    }

    public bool IsFirstBlock(GameObject block)
    {
        return firstBlock == block;
    }
}
