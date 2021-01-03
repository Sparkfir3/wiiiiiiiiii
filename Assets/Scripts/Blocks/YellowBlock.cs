using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlock : RedBlock {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [SerializeField] private bool useGravity;

    private RigidbodyConstraints heldConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    private Coroutine slowDown;

    protected override void Start() {
        base.Start();

        if(restrictX)
            heldConstraints |= RigidbodyConstraints.FreezePositionX;
        if(restrictY)
            heldConstraints |= RigidbodyConstraints.FreezePositionY;

        if(useGravity) {
            rb.useGravity = true;
            rb.constraints = heldConstraints;
        } else {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // -----------------------------------------------------------------------------------------------------------

    protected override void WhileObjectHeld() {
        base.WhileObjectHeld();
    }

    public override void OnObjectGrab() {
        base.OnObjectGrab();
        if(slowDown != null)
            StopCoroutine(slowDown);
        rb.constraints = heldConstraints;
        rb.isKinematic = false;
    }

    public override void OnObjectRelease() {
        base.OnObjectRelease();
        slowDown = StartCoroutine(SlowDown());
    }

    private IEnumerator SlowDown() {
        for(float i = 0; i < 0.5f; i += Time.deltaTime) {
            rb.velocity *= 0.9f;
            if(rb.velocity.magnitude < 0.2f)
                break;

            yield return null;
        }
        if(!useGravity) {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        } else {
            // Stop horizontal sliding
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
        yield return null;
    }
    
}
