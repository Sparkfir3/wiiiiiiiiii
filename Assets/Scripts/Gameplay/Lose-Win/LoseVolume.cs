using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseVolume : MonoBehaviour {

    [SerializeField] private float moveSpeed;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0f, 0f, moveSpeed);
    }

    private void OnTriggerEnter(Collider other) {
        GameManager.gm.LoseGame();
    }

}