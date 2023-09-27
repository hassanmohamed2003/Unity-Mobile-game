using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public void HandleCurrentBlockCollision(Collision2D collision, out bool isHardImpact, out Vector2 contactPoint)
    {
        ContactPoint2D contact = collision.contacts[0];
        contactPoint = contact.point;
        isHardImpact = contact.normalImpulse > 100;
        collision.otherRigidbody.velocity = Vector3.zero;
        collision.otherRigidbody.constraints = RigidbodyConstraints2D.None;
        collision.otherRigidbody.freezeRotation = false;
    }
}
