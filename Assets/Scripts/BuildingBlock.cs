using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    private bool ignoreCollision;

    private Collision2D target;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        if (ignoreCollision)
            return;

        if (target.gameObject.tag == "floor")
        {
            this.target = target;
            Invoke("Landed", 0f);
            ignoreCollision = true;
        }
        if (target.gameObject.tag == "Block")
        {
            this.target = target;
            Invoke("Landed", 0f);
            ignoreCollision = true;
        }
    }

    void Landed()
    {
        Game.instance.CheckHighestBlockPosition(this.target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
