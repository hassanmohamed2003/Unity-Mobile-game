using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public BlockSystem blockSystem;
    public int checkpoint = 4;
    public void FreezeCheckpointBlock()
    {
        List<GameObject> blocks = blockSystem.GetLandedBlocks();
        int index = blocks.Count - checkpoint - 1;
        if(index >= 0 && blocks[index].TryGetComponent(out Rigidbody2D rb))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.bodyType = RigidbodyType2D.Static;
        }
        UpdateCheckpoint(blocks);
    }

    private void UpdateCheckpoint(List<GameObject> blocks)
    {
        const int maxCheckpoint = 11;
        checkpoint = 4 + blocks.Count/20;
        if(checkpoint > maxCheckpoint)
        {
            checkpoint = 11;
        }
    }
}
