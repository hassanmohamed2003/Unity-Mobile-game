using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CraneController : MonoBehaviour
{
    private TouchControls touchControls;

    private Crane craneScript;
    private void Awake()
    {
        touchControls = new TouchControls();
    }
    private void OnEnable()
    {
        touchControls.Enable();
    }
    private void OnDisable()
    {
        touchControls.Disable();
    }
    private void Start()
    {
        Debug.Log("start called");
        touchControls.Touch.TouchInput.performed += ctx =>
        {
            PrimaryTouch();
        };
    }

    private void PrimaryTouch()
    {
        Debug.Log("touch called");

    }
}
