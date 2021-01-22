using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBlock : RedBlock {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Respawn")]
    [SerializeField] private float respawnTime;
    private Coroutine respawnRoutine;
    private Vector3 basePosition;

    protected override void Awake() {
        base.Awake();
        basePosition = transform.position;
    }
    
    // -----------------------------------------------------------------------------------------------------------

    public override void OnObjectGrab() {
        base.OnObjectGrab();

        if(respawnRoutine != null)
            StopCoroutine(respawnRoutine);
    }

    public override void OnObjectRelease() {
        base.OnObjectRelease();

        if(respawnRoutine != null)
            StopCoroutine(respawnRoutine);
        respawnRoutine = StartCoroutine(Respawn());
    }

    // -----------------------------------------------------------------------------------------------------------

    private IEnumerator Respawn() {
        for(float i = 0; i < respawnTime; i += Time.deltaTime)
            yield return null;
        transform.position = basePosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

}