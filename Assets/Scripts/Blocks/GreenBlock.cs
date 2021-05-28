using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlock : PassiveBlockBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float playerOffsetSpeed; // Speed to counteract the player weighing down on it
    private Vector3 targetVelocity;

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected override void WhileActive() {
        // Wiimote - accelerometer
        if(InputManager.instance.mode == InputMode.Wiimote) {
            targetVelocity = InputManager.instance.GetAccelVector() * moveSpeed;
        } else { // Mouse and keyboard - buttons
            targetVelocity = new Vector3(Input.GetAxis("Accel X"), Input.GetAxis("Accel Y"), 0f).normalized * moveSpeed;
        }
    }

    private void FixedUpdate() {
        rb.velocity = targetVelocity;
        if(PlayerController.pc.IsStandingOn(gameObject))
            rb.velocity += Vector3.up * playerOffsetSpeed;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------


}