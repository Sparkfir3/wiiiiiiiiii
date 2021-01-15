using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour {

    public bool held;
    [HideInInspector] public bool canBeHeld = true;

    [Header("Generic Block Settings")]
    public bool releaseIfPointerOffScreen;
    [SerializeField] protected bool restrictedBounds, restrictX, restrictY;

    [Header("Bounds")]
    protected List<Transform> bounds = new List<Transform>();
    protected Vector2 minPos = new Vector2(Mathf.Infinity, Mathf.Infinity), maxPos = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);

    [Header("Components")]
    protected Rigidbody rb;

    // -----------------------------------------------------------------------------------------------------------

    #region Initialization

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start() {
        if(restrictedBounds) {
            // Set parent
            GameObject parent = new GameObject() {
                name = gameObject.name + " Group"
            };
            parent.transform.position = transform.position;
            parent.transform.parent = transform.parent;
            transform.parent = parent.transform;

            // Find Children
            for(int i = 0; i < transform.childCount; i++)
                bounds.Add(transform.GetChild(i));
            // Check if enough
            if(bounds.Count < 2) {
                throw new System.ArgumentException("Not enough bounding objects for yellow block: " + gameObject.name);
            }
            // Set children and calculate min/max
            for(int i = 0; i < bounds.Count; i++) {
                bounds[i].parent = parent.transform;
                // Min
                if(bounds[i].position.x < minPos.x)
                    minPos.x = bounds[i].position.x;
                if(bounds[i].position.y < minPos.y)
                    minPos.y = bounds[i].position.y;
                // Max
                if(bounds[i].position.x > maxPos.x)
                    maxPos.x = bounds[i].position.x;
                if(bounds[i].position.y > maxPos.y)
                    maxPos.y = bounds[i].position.y;
            }

            // Check starting position
            ClampPosition();
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    protected virtual void Update() {
        if(held && releaseIfPointerOffScreen && !InputManager.instance.PointerOnScreen()) 
            OnObjectRelease();

        else if(held)
            WhileObjectHeld();

        if(restrictedBounds)
            ClampPosition();
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

    #region Misc

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

    /// <summary>
    /// Restricts the block to its min and max positions, which are determined on Start()
    /// </summary>
    protected void ClampPosition() {
        if(!restrictedBounds)
            return;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
            Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
            transform.position.z
        );
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Editor
#if UNITY_EDITOR

    public void SetBoundGizmos(bool value) {
        var bounds = GetComponentsInChildren<BoundsRenderer>();
        foreach(BoundsRenderer bound in bounds)
            bound.displayGizmo = value;
    }

#endif
    #endregion

}
