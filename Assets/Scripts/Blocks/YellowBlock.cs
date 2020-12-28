using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlock : RedBlock {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private List<Transform> bounds = new List<Transform>();
    private Vector2 minPos = new Vector2(Mathf.Infinity, Mathf.Infinity), maxPos = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);

    private static RigidbodyConstraints heldConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    private Coroutine slowDown;

    protected override void Start() {
        base.Start();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Set parent
        GameObject parent = new GameObject() {
            name = gameObject.name + " Group"
        };
        parent.transform.position = transform.position;
        parent.transform.parent = transform.parent;
        transform.parent = parent.transform;

        // Find Children
        for(int i = 0; i < transform.childCount; i++)
            bounds.Add(transform.GetChild(i));
        // Check if enough
        if(bounds.Count < 2) {
            throw new System.ArgumentException("Not enough bounding objects for yellow block: " + gameObject.name);
        }
        // Set children and calculate min/max
        for(int i = 0; i < bounds.Count; i++) {
            bounds[i].parent = parent.transform;
            // Min
            if(bounds[i].position.x < minPos.x)
                minPos.x = bounds[i].position.x;
            if(bounds[i].position.y < minPos.y)
                minPos.y = bounds[i].position.y;
            // Max
            if(bounds[i].position.x > maxPos.x)
                maxPos.x = bounds[i].position.x;
            if(bounds[i].position.y > maxPos.y)
                maxPos.y = bounds[i].position.y;
        }

        // Check starting position
        ClampPosition();
    }

    // -----------------------------------------------------------------------------------------------------------

    protected override void WhileObjectHeld() {
        base.WhileObjectHeld();
        ClampPosition();
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
        //rb.constraints = RigidbodyConstraints.FreezeAll;
        slowDown = StartCoroutine(SlowDown());
    }

    private IEnumerator SlowDown() {
        /*Vector3 referenceVelocity = rb.velocity;
        rb.velocity = Vector3.zero;*/
        for(float i = 0; i < 0.5f; i += Time.deltaTime) {
            /*transform.position += referenceVelocity * Time.deltaTime;
            referenceVelocity *= 0.75f;
            if(referenceVelocity.magnitude < 0.2f)
                break;*/
            rb.velocity *= 0.9f;
            ClampPosition();
            if(rb.velocity.magnitude < 0.2f)
                break;

            yield return null;
        }
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return null;
    }

    // -----------------------------------------------------------------------------------------------------------

    private void ClampPosition() {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
            Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
            transform.position.z
        );
    }

}
