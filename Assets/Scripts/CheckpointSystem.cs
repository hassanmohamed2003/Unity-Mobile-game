using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public BlockSystem blockSystem;
    private int checkpoint = 4;
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
        const int highScoreThreshold = 210;
        if(blocks.Count < highScoreThreshold)
        {
            checkpoint = 4 + blocks.Count/15;
        }
        else if(blocks.Count > highScoreThreshold)
        {
            int lowerScore = 4 + highScoreThreshold/15;
            checkpoint = lowerScore + (blocks.Count - highScoreThreshold)/100;
        }
    }
}
