using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSection : MonoBehaviour {

    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private bool displayCameraPos;
    private CameraController cam;

    private void Start() {
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("a");
        if(((1 >> other.gameObject.layer) & LayerMask.GetMask("Player")) != 0) {
            Debug.Log("b");
            cam.MoveToPosition(transform.position + cameraOffset);
        }
    }

    private void OnDrawGizmos() {
        if(displayCameraPos) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + cameraOffset, 0.5f);
        }
    }

}
