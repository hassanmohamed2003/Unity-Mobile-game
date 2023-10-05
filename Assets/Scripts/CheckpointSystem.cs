using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointSystem : MonoBehaviour
{
    public BlockSystem blockSystem;
    private int checkpoint = 4;
    public UnityEvent<float> RopeSwingUpdatedEvent;
    private const int maxCheckpoint = 5;
    public void FreezeCheckpointBlock()
    {
        List<GameObject> blocks = blockSystem.GetLandedBlocks();
        int index = blocks.Count - checkpoint - 1;
        if(index >= 0 && blocks[index].TryGetComponent(out Rigidbody2D rb))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.bodyType = RigidbodyType2D.Static;
        }
        if(!GameState.LockedCheckpoint)
        {
            UpdateCheckpoint(blocks);
        }
        UpdateRopeSwing();
    }

    private void UpdateCheckpoint(List<GameObject> blocks)
    {
        checkpoint = 4 + blocks.Count/20;
        if(checkpoint > maxCheckpoint)
        {
            checkpoint = maxCheckpoint;
        }
    }

    public void UpdateRopeSwing()
    {
        if(checkpoint == maxCheckpoint)
        {
            RopeSwingUpdatedEvent.Invoke(1.25f);
        }
    }
}
