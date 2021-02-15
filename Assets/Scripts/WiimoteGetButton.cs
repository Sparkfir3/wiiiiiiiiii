using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WiimoteApi;

public class WiimoteGetButton : MonoBehaviour {

    private Wiimote wiimote;

    private WiiButton[] buttonTypes;
    public Dictionary<WiiButton, bool> buttonDown = new Dictionary<WiiButton, bool>();
    private Dictionary<WiiButton, bool> buttonDownFlag = new Dictionary<WiiButton, bool>();
    public Dictionary<WiiButton, bool> buttonUp = new Dictionary<WiiButton, bool>();
    private Dictionary<WiiButton, bool> buttonUpFlag = new Dictionary<WiiButton, bool>();

    private void Awake() {
        buttonTypes = (WiiButton[])System.Enum.GetValues(typeof(WiiButton));
        foreach(WiiButton type in buttonTypes) {
            buttonDown.Add(type, false);
            buttonDownFlag.Add(type, false);
            buttonUp.Add(type, false);
            buttonUpFlag.Add(type, false);
        }
    }

    private void Update() {
        wiimote = InputManager.wiimote;
        if(wiimote == null) {
            foreach(WiiButton type in buttonTypes) {
                buttonDown[type] = false;
                buttonDownFlag[type] = false;
                buttonUp[type] = false;
                buttonUpFlag[type] = false;
            }
            return;
        }

        foreach(WiiButton type in buttonTypes) {
            // Button pressed
            if(GetCorrespondingWiimoteButton(type)) {
                // Down - check
                if(!buttonDownFlag[type]) {
                    buttonDown[type] = true;
                    buttonDownFlag[type] = true;
                } else
                    buttonDown[type] = false;

                // Up - set false
                buttonUp[type] = false;
                buttonUpFlag[type] = false;

            // Button not pressed
            } else { 
                // Down - set false
                buttonDown[type] = false;
                buttonDownFlag[type] = false;

                // Up - check
                if(!buttonUpFlag[type]) {
                    buttonUp[type] = true;
                    buttonUpFlag[type] = true;
                } else
                    buttonUp[type] = false;
            }
        }
    }

    public bool GetCorrespondingWiimoteButton(WiiButton button) {
        switch(button) {
            case WiiButton.A:
                return wiimote.Button.a;
            case WiiButton.B:
                return wiimote.Button.b;
            case WiiButton.Up:
                return wiimote.Button.d_up;
            case WiiButton.Down:
                return wiimote.Button.d_down;
            case WiiButton.Left:
                return wiimote.Button.d_left;
            case WiiButton.Right:
                return wiimote.Button.d_right;
            case WiiButton.Plus:
                return wiimote.Button.plus;
            case WiiButton.Minus:
                return wiimote.Button.minus;
            case WiiButton.Home:
                return wiimote.Button.home;
            case WiiButton.One:
                return wiimote.Button.one;
            case WiiButton.Two:
                return wiimote.Button.two;
            case WiiButton.Z:
            case WiiButton.C:
                return GetNunchuckButton(button);
            default:
                return false;
        }
    }

    private bool GetNunchuckButton(WiiButton button) {
        if(wiimote.current_ext != ExtensionController.NUNCHUCK) {
            //Debug.LogError("Nunchuck not detected");
            return false;
        }

        NunchuckData data = wiimote.Nunchuck;
        if(button == WiiButton.Z)
            return data.z;
        else if(button == WiiButton.C)
            return data.c;
        else
            return false;
    }

}