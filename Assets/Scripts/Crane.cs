using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    private Vector3 craneHalfWidth;
    private float cameraLeftBoundary;
    private float cameraRightBoundary;
    public float smooth = 0.4f;
    public Vector3 velocity = Vector3.zero;

    private Vector3 currentAcceleration, initialAcceleration;
    public float newRotation;
    public float sensitivity = 6;
    public float cameraY;
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        lastPieceDroppedTime = Time.realtimeSinceStartup;
        Input.gyro.enabled = true;
        rb = GetComponent<Rigidbody2D>();

        MainCamera = Camera.main;
        craneHalfWidth = GetComponent<Collider2D>().bounds.size;

        initialAcceleration = Input.acceleration;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuildingPiece == null && !IsReadyForNextPiece) {
            if (Time.realtimeSinceStartupAsDouble > lastPieceDroppedTime + PieceTimeInterval) {
                IsReadyForNextPiece = true;
            }
        }

        /*
        Input.gyro.enabled = true;
        float rotationRateX = Input.gyro.rotationRateUnbiased.x;

        Debug.Log(rotationRateX);
        acceleration = -rotationRateX * LeftRightSpeed * Time.deltaTime;
        */

        // // Get the tilt of the phone
        Vector3 acceleration = Input.acceleration;
        if (acceleration.x > MaxAccelerationCutoff)
        {
            acceleration.x = MaxAccelerationCutoff;

        }



        if (acceleration.x >= 0.30)
        {
            acceleration.x = 0.30f;
            acceleration = Vector3.Lerp(acceleration, Input.acceleration - acceleration, Time.deltaTime / smooth);
        }
        else if(acceleration.x <= -0.30)
        {
            acceleration.x = -0.30f;
            acceleration = Vector3.Lerp(acceleration, Input.acceleration - acceleration, Time.deltaTime / smooth);
        }



        acceleration *= Time.deltaTime * LeftRightSpeed;

        Vector3 cranePosition = transform.position;

        newRotation = Mathf.Clamp(acceleration.x * sensitivity, -0.30f, 0.30f);
        
        // Calculate the camera boundaries
        float cameraHalfWidth = MainCamera.orthographicSize * Screen.width / Screen.height;
        float cameraLeftBoundary = MainCamera.transform.position.x - cameraHalfWidth;
        float cameraRightBoundary = MainCamera.transform.position.x + cameraHalfWidth;

        float accelerationX;
        // Clamp the crane's position within the camera boundaries
        cranePosition.x = Mathf.Clamp(cranePosition.x, cameraLeftBoundary, cameraRightBoundary);

        //When Players reaches desired (L/R)possition make him stop
        if (transform.position.x <= cameraLeftBoundary)
        {
            Vector3 finalPosition;
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraLeftBoundary, 0), ref velocity, transform.position.z);
        }
        else if (transform.position.x >= cameraRightBoundary)
        {
            Vector3 finalPosition;
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraRightBoundary, 0), ref velocity, transform.position.z);
        }


        // Set the new position of the crane
        transform.position = cranePosition;
        Vector3 craneTranslation = new Vector3(acceleration.x, 0, 0);
        craneTranslation *= Time.deltaTime;
        transform.Translate(craneTranslation);

        Vector3 p = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Debug.Log(cameraY + "camera y");
        Debug.Log(craneTranslation.y + "crane y");

        if (cameraY > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, p.y, transform.position.z);
        }

        if (p.y > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, p.y, transform.position.z);
        }
        if (currentBuildingPiece != null){
            SpringJoint2D joint = currentBuildingPiece.GetComponent<SpringJoint2D>();
            joint.distance += PieceLoweringSpeed * Time.deltaTime;
        }        
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ReleaseConnectedPiece();
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
