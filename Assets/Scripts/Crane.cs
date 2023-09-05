using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crane : MonoBehaviour
{
    public float LeftRightSpeed;
    public float PieceLoweringSpeed;
    public float PieceTimeInterval;
    public float MaxAccelerationCutoff;
    public Vector2 startPos;
    private GameObject currentBuildingPiece;
    private Rigidbody2D rb;
    public bool IsReadyForNextPiece = true;
    private double lastPieceDroppedTime;

    Vector3 rot;
    Camera MainCamera;
    private float craneHalfWidth;
    private float cameraLeftBoundary;
    private float cameraRightBoundary;

    // Start is called before the first frame update
    void Start()
    {
        lastPieceDroppedTime = Time.realtimeSinceStartup;
        Input.gyro.enabled = true;
        rb = GetComponent<Rigidbody2D>();

        MainCamera = Camera.main;
        craneHalfWidth = GetComponent<Collider2D>().bounds.extents.x;




    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuildingPiece == null && !IsReadyForNextPiece) {
            if (Time.realtimeSinceStartupAsDouble > lastPieceDroppedTime + PieceTimeInterval) {
                IsReadyForNextPiece = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ReleaseConnectedPiece();
        }
        float accelerationX = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) {
            accelerationX = -1.0f * Time.deltaTime * LeftRightSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            accelerationX = 1.0f * Time.deltaTime * LeftRightSpeed;
        }


        Input.gyro.enabled = true; // Enable the gyroscope (if not already enabled)
        float rotationRateX = Input.gyro.rotationRateUnbiased.x;

        accelerationX = -rotationRateX * LeftRightSpeed * Time.deltaTime;
        // // Get the tilt of the phone
        // float accelerationX = Input.acceleration.x;
        // if (accelerationX > MaxAccelerationCutoff)
        //     accelerationX = MaxAccelerationCutoff;
        // accelerationX *= Time.deltaTime * LeftRightSpeed;

        Debug.Log(rotationRateX);

        Vector3 cranePosition = transform.position;

        // Calculate the camera boundaries
        float cameraHalfWidth = MainCamera.orthographicSize * Screen.width / Screen.height;
        float cameraLeftBoundary = MainCamera.transform.position.x - cameraHalfWidth;
        float cameraRightBoundary = MainCamera.transform.position.x + cameraHalfWidth;

        // Clamp the crane's position within the camera boundaries
        cranePosition.x = Mathf.Clamp(cranePosition.x, cameraLeftBoundary - craneHalfWidth, cameraRightBoundary + craneHalfWidth);

        //When Players reaches desired (L/R)possition make him stop
        if (transform.position.x <= cameraLeftBoundary)
            transform.position = new Vector3(cameraLeftBoundary, transform.position.y, transform.position.z);
        else if (transform.position.x >= cameraRightBoundary)
            transform.position = new Vector3(cameraRightBoundary, transform.position.y, transform.position.z);

        // Set the new position of the crane
        transform.position = cranePosition;

        // Create new vector for translating the crane
        Vector3 craneTranslation = new Vector3(accelerationX, 0, 0);
        craneTranslation *= Time.deltaTime;
        transform.Translate(craneTranslation);

        if(currentBuildingPiece != null){
            SpringJoint2D joint = currentBuildingPiece.GetComponent<SpringJoint2D>();
            joint.distance += PieceLoweringSpeed * Time.deltaTime;
        }        
    }

    public void Reset(){
        transform.position = startPos;
    }

    public void SetConnectedPiece(GameObject buildingPiece){
        currentBuildingPiece = buildingPiece;
        SpringJoint2D joint = buildingPiece.GetComponent<SpringJoint2D>();
        joint.connectedBody = rb;
        joint.anchor = new Vector2(0, 0.5f);
        joint.autoConfigureDistance = false;
        joint.distance = 1.5f;
        IsReadyForNextPiece = false;
    }

    public void ReleaseConnectedPiece(){
        if(currentBuildingPiece != null){
            SpringJoint2D joint = currentBuildingPiece.GetComponent<SpringJoint2D>();
            joint.breakForce = 0.0f;
            joint.breakTorque = 0.0f;
            lastPieceDroppedTime = Time.realtimeSinceStartupAsDouble;
            currentBuildingPiece = null;
        }
    }


}
