using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Vector3 targetPos;

    public float smoothMove = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothMove * Time.deltaTime);
    }
}
