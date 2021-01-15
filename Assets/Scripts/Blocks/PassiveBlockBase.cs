using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveBlockBase : InteractableBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Passive Block Management")]
    [SerializeField] protected bool activeOnStart;
    protected bool active;

    // -----------------------------------------------------------------------------------------------------------

    #region Initialization

    protected override void Awake() {
        base.Awake();
        canBeHeld = false;
        if(activeOnStart)
            active = true;
        else
            active = false;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected override void Update() {
        if(active)
            WhileActive();

        if(restrictedBounds)
            ClampPosition();
    }

    /// <summary>
    /// Called every Update() while the object is active
    /// </summary>
    protected abstract void WhileActive();

    // Should never be called
    protected override void WhileObjectHeld() {
        throw new System.NotImplementedException();
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------


}