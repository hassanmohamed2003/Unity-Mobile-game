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
    public UnityEvent<ContactPoint2D> ComboEvent;
    
    private int comboCounter = 0;
    public void CheckPlacement(Collision2D collision)
    {
        if (collision.rigidbody)
        {
            BoxCollider2D landingCollider = collision.otherRigidbody.gameObject.GetComponent<BoxCollider2D>();
            BoxCollider2D landedCollider = collision.rigidbody.gameObject.GetComponent<BoxCollider2D>();
            float landingBock = landingCollider.bounds.center.x;
            float landedBock = landedCollider.bounds.center.x;
            if (landedBock - landingBock > PerfectPrecision || landedBock - landingBock < -PerfectPrecision)
            {
                NormalLandingEvent.Invoke();
                comboCounter = 0;
            }
            else
            {
                PerfectLandingEvent.Invoke();
                ComboCheck(collision);
            }
        }
    }

    private void ComboCheck(Collision2D collision)
    {
        if (comboCounter < 2)
        {
            NormalComboEvent.Invoke(comboCounter);
            ComboEvent.Invoke(collision.contacts[0]);
            comboCounter++;
        }
        else if(comboCounter == 2)
        {
            MaxComboEvent.Invoke();
            ComboEvent.Invoke(collision.contacts[0]);
            comboCounter = 0;
        }
    }
}
