using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorButtonClicker : MonoBehaviour {

    public GameObject test;

    private void Update() {
        if(InputManager.instance.mode != InputMode.Wiimote)
            return;

        test = InputManager.instance.SelectedObject(LayerMask.GetMask("UI"));

        if(InputManager.instance.GetCommandDown(Command.SelectUI)) {

        }
    }

}
