using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arthur : MonoBehaviour
{
    public GameObject arthurHappy;
    public GameObject arthurSad;

    void Start()
    {
        OnCameraPositionUpdate(Camera.main);
    }

    public void OnCameraPositionUpdate(Camera cam)
    {
        // Update arthur's position based on camera's new position
        Vector3 arthurHappyNewPosition = cam.ViewportToWorldPoint(new Vector2(0.3f, 0.6f));
        arthurHappyNewPosition.x = arthurHappy.transform.position.x;
        arthurHappyNewPosition.z = 0.0f;
        UpdatePositionHappy(arthurHappyNewPosition);

        Vector3 arthurSadNewPosition = cam.ViewportToWorldPoint(new Vector2(0.3f, 0.1f));
        arthurSadNewPosition.z = 0.0f;
        UpdatePositionSad(arthurSadNewPosition);
    }
    public void UpdatePositionHappy(Vector3 position)
    {
        arthurHappy.transform.position = position;
    }

    public void UpdatePositionSad(Vector3 position)
    {
        arthurSad.transform.position = position;
    }
    public void StartHappyAnimation()
    {
        arthurHappy.SetActive(true);
    }
    public void StopHappyAnimation()
    {
        arthurHappy.SetActive(false);
    }
    public void StartSadAnimation()
    {
        arthurSad.SetActive(true);
    }
    public void StopSadAnimation()
    {
        arthurSad.SetActive(false);
    }

}
