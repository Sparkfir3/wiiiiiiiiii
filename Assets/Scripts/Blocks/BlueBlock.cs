﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBlock : PassiveBlockBase {

    [Header("Movement")]
    [SerializeField] private float jumpSpeed;
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
        if(jumpTrigger) {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            StartCoroutine(CooldownTimer());
            jumpTrigger = false;
        }
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