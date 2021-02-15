using UnityEngine;
using System.Collections;

public class RedBlock : InteractableBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Draggable")]
    [Tooltip("Speed multiplier for when object is being dragged by the pointer.")]
    [SerializeField] private float dragSpeedMultiplier;

    protected override void Update() {
        base.Update();

        // Delete if out of bounds
        if(transform.position.y <= -5f)
            Destroy(gameObject);
    }

    protected override void WhileObjectHeld() {
        Vector3 direction = GetPositionFromPointer() - transform.position;
        rb.velocity = direction * dragSpeedMultiplier;
    }

}
