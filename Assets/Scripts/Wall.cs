using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Camera mainCam;
    public float WallEdgeOffset;
    private float leftEdgeScreenX, rightEdgeScreenX;
    
    // Start is called before the first frame update
    void Start()
    {
        float halfWidth = transform.localScale.x / 2.0f;
        leftEdgeScreenX = mainCam.ViewportToWorldPoint(new Vector3(0,0,0)).x;
        rightEdgeScreenX = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 wallPosition = rb.position;

        if(wallPosition.x < 0){
            wallPosition.x = leftEdgeScreenX - halfWidth - WallEdgeOffset;
        }else{
            wallPosition.x = rightEdgeScreenX + halfWidth + WallEdgeOffset;
        }

        transform.position = wallPosition;     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
