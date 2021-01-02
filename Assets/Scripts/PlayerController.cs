using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpStrength, floatGravity, shorthopGravity, fallGravity, maxFallSpeed;
    [SerializeField] private bool jumpFlag; // Serialized for testing
    [SerializeField] private bool grounded; // Declared ONLY for viewing in inspector
    private CharacterController controller;

    [Header("Object Control")]
    private GameObject heldObject;
    private InteractableBase interactable;

    [Header("Misc")]
    private static LayerMask blockMask;

    private void Awake() {
        controller = GetComponent<CharacterController>();
        if(maxFallSpeed < 0)
            maxFallSpeed *= -1f;
        jumpFlag = false;

        blockMask = LayerMask.GetMask("Block");
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Repeating

    private void Update() {
        // No wiimote
        if(InputManager.instance.mode == InputMode.Wiimote && InputManager.wiimote == null)
            return;

        // ---

        #region Jump Detection

        if(!jumpFlag && InputManager.instance.GetCommandDown(Command.Jump))
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
                if(interactable) {
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
        Vector3 newVelocity = new Vector3(InputManager.instance.GetAxis(Axis.Horizontal), 0f, InputManager.instance.GetAxis(Axis.Vertical)) * moveSpeed;
        if(jumpFlag) {
            newVelocity.y = jumpStrength;
            jumpFlag = false;
        } else {
            newVelocity.y = Mathf.Clamp(controller.velocity.y - FindGravity(), -maxFallSpeed, Mathf.Infinity);
        }
        controller.Move(newVelocity * Time.deltaTime);

        grounded = controller.isGrounded;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Collisions

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(((1 << hit.gameObject.layer) & blockMask) != 0) {
            //Debug.Log("test");
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Movement

    /// <summary>
    /// Returns the player's current gravity as a float value
    /// </summary>
    private float FindGravity() {
        if(controller.velocity.y > 0) {
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
