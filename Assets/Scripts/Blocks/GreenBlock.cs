using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlock : PassiveBlockBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private Vector3 targetVelocity;

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected override void WhileActive() {
        // Wiimote - accelerometer
        if(InputManager.instance.mode == InputMode.Wiimote) {
            targetVelocity = InputManager.instance.GetAccelVector() * moveSpeed;
        } else { // Mouse and keyboard - buttons
            // TODO
            targetVelocity = Vector3.up;
        }
    }

    private void FixedUpdate() {
        rb.velocity = targetVelocity;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------


}