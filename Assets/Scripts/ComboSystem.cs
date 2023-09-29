using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComboSystem : MonoBehaviour
{
    [Header("Perfect Precision")]
    public float PerfectPrecision;
    public UnityEvent PerfectLandingEvent;
    public UnityEvent NormalLandingEvent;
    public UnityEvent<int> NormalComboEvent;
    public UnityEvent MaxComboEvent;
    
    private int comboCounter = 0;
    public void CheckPlacement(Collision2D collision)
    {
        if (collision.rigidbody)
        {
            float landingBock = collision.otherRigidbody.transform.position.x;
            float landedBock = collision.rigidbody.transform.position.x;
            if (landedBock - landingBock > PerfectPrecision || landedBock - landingBock < -PerfectPrecision)
            {
                NormalLandingEvent.Invoke();
                comboCounter = 0;
            }
            else
            {
                PerfectLandingEvent.Invoke();
                ComboCheck();
            }
        }
    }

    private void ComboCheck()
    {
        if (comboCounter < 2)
        {
            NormalComboEvent.Invoke(comboCounter);
            comboCounter++;
        }
        else if(comboCounter == 2)
        {
            MaxComboEvent.Invoke();
            comboCounter = 0;
        }
    }
}
