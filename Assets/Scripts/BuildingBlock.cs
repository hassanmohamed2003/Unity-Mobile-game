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
    private float currentRotation;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.CompareTag("Landed Block") && ignoreCollision)
        {
            return;
        }
        transform.gameObject.tag = "Landed Block";
        this.target = target;
        Landed();
        ignoreCollision = true;
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
}
