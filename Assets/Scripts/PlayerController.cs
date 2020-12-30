using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject heldObject;
    private InteractableBase interactable;

    private void Update() {
        if(InputManager.instance.mode == InputMode.Wiimote && InputManager.wiimote == null) // No wiimote
            return;

        // ---

        // Look for object
        if(!heldObject) { 
            // Find selected object
            GameObject selectedObj = InputManager.instance.SelectedObject(LayerMask.GetMask("Block"));
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
    }

    public void ReleaseObject() {
        if(heldObject) {
            heldObject.GetComponent<InteractableBase>().OnObjectRelease();
            heldObject = null;
            interactable = null;
        }
    }

}
