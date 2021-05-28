using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Ground Detection")]
    [SerializeField] private GameObject currentGround;
    [SerializeField] private Vector3 offset, size;
    private PlayerController player;
    private LayerMask mask;

    [Header("Debug")]
    [SerializeField] private bool displayGizmo = true;
    private bool infiniteJumps = false;

    private void Awake() {
        player = GetComponentInParent<PlayerController>();
        mask = LayerMask.GetMask("Terrain", "Block", "Block Restricted", "Block Alt");
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.BackQuote)) {
            infiniteJumps = !infiniteJumps;
            Debug.Log("Infinite Jumps set to " + infiniteJumps);
        }
    }

    private void FixedUpdate() {
        if(Physics.BoxCast(transform.position + offset, new Vector3(size.x / 2f, 0.01f, size.z / 2f), Vector3.down, out RaycastHit hit, Quaternion.identity, size.y, mask)) {
            currentGround = hit.collider.gameObject;
        } else {
            currentGround = null;
        }
        player.CurrentGround = currentGround;

        if(infiniteJumps && !player.CurrentGround)
            player.CurrentGround = gameObject;
    }

    private void OnDrawGizmos() {
        if(displayGizmo) {
            Gizmos.color = Color.green;
            Vector3 gizmoCenter = transform.position + offset;
            Gizmos.DrawWireCube(gizmoCenter - new Vector3(0, size.y / 2), size);
            Gizmos.DrawSphere(gizmoCenter, 0.05f);

            /*Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(gizmoCenter, size);*/
        }
    }

}