using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockLong : MonoBehaviour
{
    private bool ignoreCollision;
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
            Invoke("Landed", 2f);
            ignoreCollision = true;
        }
        if (target.gameObject.tag == "Block")
        {
            Invoke("Landed", 2f);
            ignoreCollision = true;
        }
    }

    void Landed()
    {
        Game.instance.MoveCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
