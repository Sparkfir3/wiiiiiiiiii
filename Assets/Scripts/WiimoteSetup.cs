using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteSetup : MonoBehaviour {

    public bool FindWiimote() {
        if(InputManager.instance.mode == InputMode.Wiimote)
            Debug.Log("Finding wiimote...");
        WiimoteManager.FindWiimotes();

        if(WiimoteManager.HasWiimote()) {
            InputManager.wiimote = WiimoteManager.Wiimotes[0];
            InputManager.wiimote.SendPlayerLED(true, false, false, false);

            // Set input mode -> acceleration + extensions
            InputManager.wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);

            // Set IR
            InputManager.wiimote.SetupIRCamera(IRDataType.EXTENDED);

            Debug.Log("Wiimote found and setup");
            return true;
        } else {
            InputManager.wiimote = null;
            return false;
        }
    }
	
}
