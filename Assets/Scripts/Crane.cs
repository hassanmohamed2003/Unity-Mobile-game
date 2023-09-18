using System;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [Header("Movement")]
    public float LeftRightSpeed;
    public float PieceLoweringSpeed;
    public float PieceTimeInterval;
    public float MaxAcceleration;

    [Header("Camera")]
    public Camera mainCam;

    [Header("Behaviour")]
    public bool IsReadyForNextPiece = true;
    public float pieceStartHeight;
    public Vector2 ropeStartOffset;
    public float InitialSwingForce;
    private double lastPieceDroppedTime;
    private GameObject currentBuildingPiece;
    private Rigidbody2D rb;
    private float leftEdgeScreenX, rightEdgeScreenX;
    private LineRenderer lineRenderer;
    public bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        lastPieceDroppedTime = Time.realtimeSinceStartup;
        Input.gyro.enabled = true;
        rb = GetComponent<Rigidbody2D>();

        // Get the edge of the screen
        leftEdgeScreenX = mainCam.ViewportToWorldPoint(new Vector3(0,0,0)).x;
        rightEdgeScreenX = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        // Check if crane can start handling a new piece
        if (currentBuildingPiece == null && !IsReadyForNextPiece) {
            if (Time.realtimeSinceStartupAsDouble > lastPieceDroppedTime + PieceTimeInterval) {
                // If it is allowed let Game know
                IsReadyForNextPiece = true;
            }
        }

        // Get the tilt of the phone
        float accelerationX = Math.Clamp(Input.acceleration.x, -MaxAcceleration, MaxAcceleration);

        accelerationX *= Time.deltaTime * LeftRightSpeed;

        // Get the top of the camera screen
        float translationY = mainCam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        translationY -= rb.position.y;

        // Build translation vector for this frame
        Vector2 craneTranslation = new(accelerationX, translationY);

        // Override tilt controls for testing with keyboard
        if(Input.GetKey(KeyCode.A)){
            craneTranslation.x = 0.3f * -LeftRightSpeed;   
        }
        else if(Input.GetKey(KeyCode.D)){
            craneTranslation.x = 0.3f * LeftRightSpeed;
        }
        
        // Move crane
        Vector2 newCranePosition = rb.position + craneTranslation;
        newCranePosition.x = Math.Clamp(newCranePosition.x, leftEdgeScreenX, rightEdgeScreenX);

        if (!isGameOver)
        {
            rb.MovePosition(newCranePosition);
        }

        // Release building piece when touch input is detected
        if(Input.touchCount > 0) ReleaseConnectedPiece();
        if(Input.GetKeyDown(KeyCode.R)) ReleaseConnectedPiece();

        if (currentBuildingPiece != null){
            SpringJoint2D joint = currentBuildingPiece.GetComponent<SpringJoint2D>();
            joint.distance += PieceLoweringSpeed * Time.deltaTime;

            Vector2 newRopeStartPos = (Vector2)transform.position - joint.anchor;
            Vector2 newRopeEndPos = currentBuildingPiece.transform.position;
            if(currentBuildingPiece.TryGetComponent(out BuildingBlock block)){
                newRopeEndPos.y += block.ropeStartOffset;
            }           
            
            lineRenderer.SetPosition(0, newRopeStartPos);
            lineRenderer.SetPosition(1, newRopeEndPos);
        }        
    }

    public void SetConnectedPiece(GameObject buildingPiece){
        currentBuildingPiece = buildingPiece;
        SpringJoint2D joint = buildingPiece.GetComponent<SpringJoint2D>();
        joint.connectedBody = rb;
        joint.anchor = ropeStartOffset;
        joint.autoConfigureDistance = false;
        joint.distance = pieceStartHeight; 

        lineRenderer.forceRenderingOff = false;

        Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();
        Vector3 rbPosition = rb.position;
        rbPosition.y -= 2;
        rigidbody.MovePosition(rb.position);

        Rigidbody2D rigidbodyPiece = currentBuildingPiece.GetComponent<Rigidbody2D>();
        rigidbodyPiece.AddForce(Vector2.right * InitialSwingForce, ForceMode2D.Force);
        IsReadyForNextPiece = false;
    }

    public void ReleaseConnectedPiece(){
        if(currentBuildingPiece != null){
            SpringJoint2D joint = currentBuildingPiece.GetComponent<SpringJoint2D>();
            joint.breakForce = 0.0f;
            joint.breakTorque = 0.0f;
            lastPieceDroppedTime = Time.realtimeSinceStartupAsDouble;

            Rigidbody2D rb = currentBuildingPiece.GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.None;
            rb.freezeRotation = false;
            currentBuildingPiece = null;

            // Hide line
            lineRenderer.forceRenderingOff = true;
        }
    }


}
