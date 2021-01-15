using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsRenderer : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [SerializeField] private GameObject model;
    [Range(-5f, 5f)] [SerializeField] private float visualOffsetX, visualOffsetY, visualOffsetZ;

#if UNITY_EDITOR
    private Vector3 visualOffset;
    public bool displayGizmo;
#endif

    private void Start() {
#if UNITY_EDITOR
        displayGizmo = false;
#endif

        transform.localScale = Vector3.one;
        Instantiate(model, transform.position + new Vector3(visualOffsetX, visualOffsetY, visualOffsetZ), Quaternion.Euler(-90f, -90f, 0f), transform);
    }

    private void OnDrawGizmos() {
        if(displayGizmo) {
            Gizmos.color = Color.gray;
            visualOffset = new Vector3(visualOffsetX, visualOffsetY, visualOffsetZ);
            Gizmos.DrawSphere(transform.position + visualOffset, 0.25f);
            if(!visualOffset.Equals(Vector3.zero)) {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(transform.position, 0.15f);
            }
        }
    }

}