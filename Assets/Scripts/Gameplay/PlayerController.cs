using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpStrength, floatGravity, shorthopGravity, fallGravity, maxFallSpeed;
    [SerializeField] private bool jumpFlag; // Serialized for testing
    public GameObject currentGround;
    private Rigidbody rb;

    [Header("Object Control")]
    private GameObject heldObject;
    private InteractableBase interactable;

    [Header("Misc")]
    private static LayerMask blockMask, movingBlockMask;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        if(maxFallSpeed < 0)
            maxFallSpeed *= -1f;
        jumpFlag = false;

        blockMask = LayerMask.GetMask("Default", "Block", "Block Restricted", "Block Alt");
        movingBlockMask = LayerMask.GetMask("Block Restricted", "Block Alt");
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    private void Update() {
        // No wiimote
        if(InputManager.instance.mode == InputMode.Wiimote && InputManager.wiimote == null)
            return;

        // ---

        #region Jump Detection

        if(!jumpFlag && currentGround && InputManager.instance.GetCommandDown(Command.Jump))
            jumpFlag = true;

        #endregion

        // ---

        #region Object Grabbing

        // Look for object
        if(!heldObject) { 
            // Find selected object
            GameObject selectedObj = InputManager.instance.SelectedObject(blockMask);
            // TODO - shader set while hovering over

            // If button pressed, pick up object
            if(selectedObj != null && InputManager.instance.GetCommandDown(Command.SelectObj)) {
                interactable = selectedObj.GetComponent<InteractableBase>();
                if(interactable && interactable.canBeHeld) {
                    heldObject = selectedObj;
                    interactable.OnObjectGrab();
                }
            }
        } else {
            // If button is released while object is held, release it
            if(InputManager.instance.GetCommandUp(Command.SelectObj))
                ReleaseObject();

            // Check if object released itself
            if(!interactable || !interactable.held) {
                heldObject = null;
                interactable = null;
            }
        }

        #endregion
    }

    private void FixedUpdate() {
        // Get inputs
        Vector3 newVelocity = new Vector3(InputManager.instance.GetAxis(Axis.Horizontal), 0f, InputManager.instance.GetAxis(Axis.Vertical)) * moveSpeed;

        // Apply jump
        if(jumpFlag) {
            newVelocity.y = jumpStrength;
            jumpFlag = false;
        } else {
            newVelocity.y = Mathf.Clamp(rb.velocity.y - FindGravity(), -maxFallSpeed, Mathf.Infinity);
        }

        // Apply moving platforms
        if(currentGround && ((1 << currentGround.layer) & movingBlockMask) != 0) {
            Rigidbody blockRb = currentGround.GetComponent<Rigidbody>();
            if(blockRb) {
                newVelocity += new Vector3(blockRb.velocity.x, 0f, blockRb.velocity.z);
            } else
                Debug.LogError("Attempting to grab Rigidbody component from a block without a Rigidbody component: " + currentGround.name);
        }

        // Apply total velocity
        rb.velocity = newVelocity;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Movement

    /// <summary>
    /// Returns the player's current gravity as a float value
    /// </summary>
    private float FindGravity() {
        if(rb.velocity.y > 0) {
            if(InputManager.instance.GetCommand(Command.Jump)) {
                return floatGravity;
            } else {
                return shorthopGravity;
            }
        }
        return fallGravity;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Object Selection

    public void ReleaseObject() {
        if(heldObject) {
            heldObject.GetComponent<InteractableBase>().OnObjectRelease();
            heldObject = null;
            interactable = null;
        }
    }

    #endregion

}
