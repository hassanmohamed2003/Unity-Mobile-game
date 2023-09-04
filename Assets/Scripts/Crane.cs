using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public float LeftRightSpeed;
    public float PieceLoweringSpeed;
    public float MaxAccelerationCutoff;
    public Transform Camera;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;   
    }

    // Update is called once per frame
    void Update()
    {
        // Get the tilt of the phone
        float accelerationX = Input.acceleration.x;
        if (accelerationX > MaxAccelerationCutoff)
            accelerationX = MaxAccelerationCutoff;
        accelerationX *= Time.deltaTime * LeftRightSpeed;

        // Create new vector for translating the crane
        Vector3 craneTranslation = new Vector3(accelerationX, -PieceLoweringSpeed, 0);
        craneTranslation *= Time.deltaTime;
        transform.Translate(craneTranslation);

        // Update camera position
        Camera.Translate(craneTranslation);
    }
}
