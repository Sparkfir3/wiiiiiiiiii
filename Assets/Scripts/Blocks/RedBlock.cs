using UnityEngine;
using System.Collections;

public class RedBlock : InteractableBase {
    
    [Tooltip("Speed multiplier for when object is being dragged by the pointer.")]
    [SerializeField] private float dragSpeedMultiplier;

    protected override void WhileObjectHeld() {
        float offsetFromCamera = GetOffsetFromCamera();
        Vector3 direction = InputManager.instance.PointerToWorldPos(offsetFromCamera) - transform.position;
        rb.velocity = direction * dragSpeedMultiplier;
    }

    public override void OnObjectRelease() {
        base.OnObjectRelease();
    }

}
