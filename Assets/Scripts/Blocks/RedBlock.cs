using UnityEngine;
using System.Collections;

public class RedBlock : InteractableBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Tooltip("Speed multiplier for when object is being dragged by the pointer.")]
    [SerializeField] private float dragSpeedMultiplier;

    protected override void WhileObjectHeld() {
        Vector3 direction = GetPositionFromPointer() - transform.position;
        rb.velocity = direction * dragSpeedMultiplier;
    }

}
