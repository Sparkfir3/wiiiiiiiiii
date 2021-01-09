using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextSection : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    public UnityEvent OnFirstTrigger;
    private bool triggered;

    [SerializeField] private Vector3 cameraOffset;
    private CameraController cam;

    [Header("Debug")]
    [SerializeField] private bool displayCameraPos;


    // -----------------------------------------------------------------------------------------------------------

    private void Start() {
        cam = Camera.main.GetComponent<CameraController>();

        triggered = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) != 0) {
            if(!cam)
                cam = Camera.main.GetComponent<CameraController>();
            JumpCamera();

            if(!triggered) {
                OnFirstTrigger.Invoke();
                triggered = true;
            }
        }
    }

    public void JumpCamera() {
        cam.MoveToPosition(transform.position + cameraOffset, this);
    }

    // -----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

    private void OnDrawGizmos() {
        if(displayCameraPos) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + cameraOffset, 0.25f);
        }
    }

    public void PrintTargetCameraPos() {
        Debug.Log(transform.position + cameraOffset);
    }

#endif

}
