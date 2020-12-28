using UnityEngine;
using System.Collections;

public abstract class InteractableBase : MonoBehaviour {

    public bool held;
    public bool releaseIfPointerOffScreen;

    protected Rigidbody rb;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start() { }

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

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Object Grab and Release

    /// <summary>
    /// Called when the object is grabbed by the player
    /// </summary>
    public virtual void OnObjectGrab() {
        held = true;
    }

    /// <summary>
    /// Called when the object is released by the player
    /// </summary>
    public virtual void OnObjectRelease() {
        held = false;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Returns the target position relative to the pointer in the proper xy-plane
    /// See Notion document for calculations and full formula
    /// </summary>
    protected Vector3 GetPositionFromPointer() {
        Vector3 direction = InputManager.instance.GetPointerDirectionVector();
        Vector3 cameraPos = Camera.main.transform.position;

        float n = (transform.position.z - cameraPos.z) / direction.z;
        float x = cameraPos.x + (n * direction.x);
        float y = cameraPos.y + (n * direction.y);
        return new Vector3(x, y, transform.position.z);
    }

}
