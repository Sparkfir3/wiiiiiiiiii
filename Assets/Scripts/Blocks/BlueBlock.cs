using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBlock : PassiveBlockBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Movement")]
    [SerializeField] private float jumpSpeed;
    [Range(-90, 90)] [SerializeField] private float angularSpeedX, angularSpeedY, angularSpeedZ;
    [SerializeField] private float playerOffsetSpeed, playerOffsetJumpSpeed; // Speed to counteract the player weighing down on it
    [SerializeField] private float jumpCooldown;
    private float cooldownTimer;
    private bool jumpTrigger = false;

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected override void WhileActive() {
        if(cooldownTimer == 0 && InputManager.instance.GetCommand(Command.Shake)) {
            jumpTrigger = true;
        }
    }

    private void FixedUpdate() {
        bool playerStandingOn = PlayerController.pc.IsStandingOn(gameObject);

        if(jumpTrigger) {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            if(playerStandingOn)
                rb.velocity += Vector3.up * playerOffsetJumpSpeed;
            rb.angularVelocity = new Vector3(angularSpeedX, angularSpeedY, angularSpeedZ);
            StartCoroutine(CooldownTimer());
            jumpTrigger = false;
        }

        if(playerStandingOn)
            rb.velocity += Vector3.up * playerOffsetSpeed;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    private IEnumerator CooldownTimer() {
        for(cooldownTimer = jumpCooldown; cooldownTimer > 0; cooldownTimer -= Time.deltaTime) {
            yield return null;
        }
        cooldownTimer = 0;
    }

}