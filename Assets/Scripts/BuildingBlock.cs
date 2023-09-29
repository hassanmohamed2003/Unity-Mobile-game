using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BlockCollisionEvent : UnityEvent<Collision2D>
{
}

public class BuildingBlock : MonoBehaviour
{
    public BlockSystem blockSystem;
    public float ropeStartOffset;
    private bool ignoreCollision;

    private void OnCollisionEnter2D(Collision2D target)
    {
        if ((target.gameObject.TryGetComponent(out BuildingBlock _) && !ignoreCollision)
         || (target.gameObject.TryGetComponent(out Floor _) && !blockSystem.HasFirstBlockLanded))
        {
            ignoreCollision = true;
            blockSystem.OnBlockCollision(target);
        }
        else if (target.gameObject.TryGetComponent(out Floor _) && !Game.instance.IsGameOver)
        {
            Game.instance.BlockHitFloor();
        }       
    }
}
