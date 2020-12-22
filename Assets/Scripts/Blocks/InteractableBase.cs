using UnityEngine;
using System.Collections;

public abstract class InteractableBase : MonoBehaviour {

	public bool held;
    public bool releaseIfPointerOffScreen;

    protected Rigidbody rb;

    protected void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected void Update() {
        if(releaseIfPointerOffScreen && !InputManager.instance.PointerOnScreen()) {
            held = false;
            OnObjectRelease();
        }

        else if(held)
            WhileObjectHeld();
    }

    /// <summary>
    /// Called while the object is being held by the player
    /// </summary>
    protected abstract void WhileObjectHeld();

    /// <summary>
    /// Called when the object is released by the player
    /// </summary>
    public virtual void OnObjectRelease() {
        held = false;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    protected float GetOffsetFromCamera() {
        Vector3 cameraPos = Camera.main.transform.position;
        return Mathf.Abs(cameraPos.z - transform.position.z);
    }

}
