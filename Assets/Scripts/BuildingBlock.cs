using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    private bool ignoreCollision;
    float counter;
    private Collision2D target;
    public float ropeStartOffset;

    private bool hasLanded = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        if (ignoreCollision)
            return;

        if (target.gameObject.CompareTag("Floor"))
        {
            transform.gameObject.tag = "Landed Block";
            this.target = target;
            Invoke("Landed", 0f);

        }
        else if (target.gameObject.CompareTag("Landed Block"))
        {
            transform.gameObject.tag = "Landed Block";
            this.target = target;
            Invoke("Landed", 0f);

        }
    }

    void Landed()
    {
        counter++;
        Game.instance.HasDropped(this.target);
        hasLanded = true;
    }

    public bool HasLanded
    {
        get { return hasLanded; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
