using UnityEngine;
using System.Collections;

public class RedBlock : InteractableBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Draggable")]
    [Tooltip("Speed multiplier for when object is being dragged by the pointer.")]
    [SerializeField] private float dragSpeedMultiplier;
    protected Vector3 desiredVelocity;

    protected override void WhileObjectHeld() {
        Vector3 direction = GetPositionFromPointer() - transform.position;
        desiredVelocity = direction * dragSpeedMultiplier;
        rb.velocity = desiredVelocity;
    }

    protected virtual void OnCollisionStay(Collision collision) {
        if(held) {
            if(((1 << collision.gameObject.layer) & LayerMask.GetMask("Player")) != 0) {
                Debug.Log(collision.contactCount);
                player.collidePos = collision.GetContact(0).point;
                player.pushedVelocity = new Vector3(desiredVelocity.x, desiredVelocity.y > 0f ? desiredVelocity.y : 0f, desiredVelocity.z);
            } else {
                player.pushedVelocity = Vector3.zero;
            }
        }
    }

}
