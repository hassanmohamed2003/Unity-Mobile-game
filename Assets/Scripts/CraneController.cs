using UnityEngine;
using UnityEngine.InputSystem;

public class CraneController : MonoBehaviour
{

    public InputAction playerFire;

    public GameObject projectilePrefab;

    public Crane craneScript;
    public Transform firePoint;

    private void Awake()
    {

        playerFire.Enable();
        playerFire.performed -= ctx => Fire();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        playerFire.Disable();
        playerFire.performed -= ctx => Fire();
    }

    private void Update()
    {
        
    }
    // Method to fire a projectile.
    private void Fire()
    {
        craneScript.ReleaseConnectedPiece();
    }
}
