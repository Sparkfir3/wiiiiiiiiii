using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject heldObject;
    private InteractableBase interactable;

    private void Update() {
        if(InputManager.instance.mode == InputManager.InputMode.Wiimote && InputManager.wiimote == null) // No wiimote
            return;

        // ---

        if(!heldObject) { // Look for object
            // Find selected object
            GameObject selectedObj = InputManager.instance.SelectedObject(LayerMask.GetMask("Default"));
            // TODO - shader set while hovering over

            // If button pressed, pick up object
            if(selectedObj != null && InputManager.wiimote.Button.b) {
                interactable = selectedObj.GetComponent<InteractableBase>();
                if(interactable) {
                    heldObject = selectedObj;
                    interactable.held = true;
                }
            }
        } else {
            // If button is released while object is held, release it
            if(!InputManager.wiimote.Button.b)
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
