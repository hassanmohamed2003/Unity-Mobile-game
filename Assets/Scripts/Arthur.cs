using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arthur : MonoBehaviour
{
    public GameObject arthurHappy;
    public GameObject arthurSad;
    public void UpdatePosition(Vector3 position)
    {
        position.z = 0.0f;
        arthurHappy.transform.position = position;
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
