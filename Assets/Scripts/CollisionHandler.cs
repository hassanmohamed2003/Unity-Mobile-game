using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    [Range(0.0f, 1000.0f)]
    public float HardImpactThreshold;
    public UnityEvent<ContactPoint2D> HardBlockImpactEvent;
    public void HandleCurrentBlockCollision(Collision2D collision)
    {
        // Get the first contact point
        // This is fine because colliding boxes only have one contact point
        ContactPoint2D contact = collision.contacts[0];

        // Reset velocity and update constraints
        collision.otherRigidbody.velocity = Vector3.zero;
        collision.otherRigidbody.constraints = RigidbodyConstraints2D.None;
        collision.otherRigidbody.freezeRotation = false;

        // Check if impact was "hard"
        if(contact.normalImpulse >= HardImpactThreshold)
        {
            // If it was, let other systems know
            HardBlockImpactEvent.Invoke(contact);
        }
    }
}
