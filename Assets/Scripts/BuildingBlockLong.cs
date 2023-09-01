using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockLong : MonoBehaviour
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

        if (target.gameObject.tag == "Floor")
        {
            this.target = target;
            Invoke("Landed",0f);
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
