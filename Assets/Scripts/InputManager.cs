using UnityEngine;
using System.Collections;
using WiimoteApi;

public class InputManager : MonoBehaviour {

    public enum InputMode {Wiimote, MouseKeyboard};
	public static InputManager instance;
    public static Wiimote wiimote;

    public InputMode mode;

    [Header("Object References")]
    [HideInInspector] public WiimoteSetup wiimoteSetup;

    [Header("Pointer")]
    [SerializeField] private RectTransform pointer;
    [HideInInspector] public Camera cam;

    [Header("Shake Detection")]
    private bool _shaking;
    private Vector3 prevAccelValue;
    private float[] prevAccelAngles = new float[5];

    private void Start() {
        cam = Camera.main; // TODO - update dynamically with scene
    }

    // -----------------------------------------------------------------------------------------------------------

    private void Update() {
        // Find wiimote
        if(!WiimoteManager.HasWiimote()) {
            if(!wiimoteSetup.FindWiimote())
                return; // Exit if no wiimote found
        }

        // ---

        // Read data
        int ret;
        do {
            ret = wiimote.ReadWiimoteData();
        } while(ret > 0);

        // ---

        // Pointer
        float[] pointerData = wiimote.Ir.GetPointingPosition();
        Vector2 pointerPos = GetStabilizdePointerPos(pointer.anchorMax, new Vector2(pointerData[0], pointerData[1]));
        pointer.anchorMax = pointerPos;
        pointer.anchorMin = pointerPos;

        // ---

        // Shake
        if(Time.frameCount % 2 == 0) { // Every 2 frames

        }
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Pointer

    /// <summary>
    /// Returns a lerped/stabilized position for the pointer
    /// </summary>
    /// <param name="basePos">The pointer's current position</param>
    /// <param name="newPos">The position the pointer is attempting to go towards</param>
    private Vector3 GetStabilizdePointerPos(Vector3 basePos, Vector3 newPos) {
        float distance = (newPos - basePos).magnitude;

        if(distance < 0.03f)
            return Vector2.Lerp(basePos, newPos, 0.3f);
        else if(distance < 0.05f)
            return Vector2.Lerp(basePos, newPos, 0.7f);
        return newPos;
    }
    
    /// <summary>
    /// Returns the world position corresponding to the pointer with the given offset
    /// </summary>
    /// <param name="forwardOffset">Units forward from the camera the world position is</param>
    public Vector3 PointerToWorldPos(float forwardOffset) {
        if(pointer.anchorMin == new Vector2(-1f, -1f))
            return Vector3.zero;

        return cam.ViewportToWorldPoint(new Vector3(pointer.anchorMin.x, pointer.anchorMin.y, forwardOffset));
    }

    /// <summary>
    /// Returns the current object the pointer is aiming at
    /// </summary>
    /// <param name="mask">The LayerMask used for the detection raycast</param>
    /// <param name="maxDistance">Maximum distance to check</param>
    public GameObject SelectedObject(LayerMask mask, float maxDistance = 15f) {
        Vector3 pointerPos = cam.ViewportToWorldPoint(new Vector3(pointer.anchorMin.x, pointer.anchorMin.y, Camera.main.nearClipPlane));
        Vector3 direction = (pointerPos - cam.transform.position).normalized;

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, direction, out hit, maxDistance)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Returns whether or not the pointer is currently on screen
    /// </summary>
    public bool PointerOnScreen() {
        return pointer.anchorMax[0] != -1f;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Accelerometer

    /// <summary>
    /// Returns the wiimote's acceleration data as a vector, normalized.
    /// </summary>
    public Vector3 GetAccelVector() {
        return GetAccelVectorRaw().normalized;
    }

    /// <summary>
    /// Returns the wiimote's acceleration data as a vector.
    /// </summary>
    public Vector3 GetAccelVectorRaw() {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData();
        accel_x = accel[0];
        accel_y = accel[2];
        accel_z = accel[1];

        return new Vector3(accel_x, accel_y, accel_z);
    }

    // ------------------------

    private void CalculateShake() {
        // Calculate
        Vector3 nextAccel = GetAccelVector();
        float angle = Vector3.Angle(nextAccel, prevAccelValue);
        bool[] flags = new bool[prevAccelAngles.Length - 1];

        for(int i = 0; i < prevAccelAngles.Length - 1; i++) {
            if(prevAccelAngles[i] < 60) {
                for(int j = 0; j < flags.Length; j++) {
                    if(!flags[j]) {
                        flags[j] = true;
                        break;
                    }
                }
            }
        }

        // Determine shaking
        _shaking = !flags[flags.Length - 1];

        // Update accel and angles
        for(int i = 0; i < prevAccelAngles.Length - 1; i++) {
            prevAccelAngles[i] = prevAccelAngles[i + 1];
        }
        prevAccelValue = nextAccel;
        prevAccelAngles[prevAccelAngles.Length - 1] = angle;
    }

    /// <summary>
    /// Whether or not the wiimote is shaking.
    /// </summary>
    public bool Shake {
        get {
            return _shaking;
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Nunchuck

    /// <summary>
    /// Returns a value from -1.0f to 1.0f, representing the joystick's position in the given axis
    /// </summary>
    /// <param name="axis">The axis the check for, either "Horizontal" or "Vertical"</param>
    public float GetNunchuckAxis(string axis) {
        if(wiimote.current_ext != ExtensionController.NUNCHUCK) {
            Debug.LogError("Nunchuck not detected");
            return 0;
        }

        NunchuckData data = wiimote.Nunchuck;
        int value = 0;
        switch(axis) {
            case "Horizontal":
                value = data.stick[0]; // General range is 35-228
                break;
            case "Vertical":
                value = data.stick[1]; // General range is 27-220
                break;
            default:
                throw new System.ArgumentException("Invalid argument: " + axis + ", expected \"Horizontal\" or \"Vertical\"");
        }

        // Check if input mode not setup
        if(value == 0) {
            Debug.LogError("Attempting to read nunchuck data when input mode is not setup");
            return 0f;
        }

        // Dead zone - Center is around 128, range of 16
        if(value > 112 && value < 144)
            return 0f;

        // Set horizontal to similar range as vertical
        if(axis == "Horizontal")
            value -= 8;

        // Check for upper/lower bounds
        if(value > 200)
            return 1f;
        else if(value < 47)
            return -1f;

        // Return normalized value
        float normalizedValue = (value - 128f) / 128f;
        return Mathf.Clamp(normalizedValue, -1f, 1f);
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    private void OnApplicationQuit() {
        if(wiimote != null) {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }
    }

}
