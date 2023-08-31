using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource currentSoundSource;
    public Vector2 startPosition;

    public BuildingBlock(Vector2 startPos){
        this.startPosition = startPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSoundSource = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
