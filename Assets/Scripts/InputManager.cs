using UnityEngine;
using System.Collections;
using WiimoteApi;

public enum InputMode { Wiimote, MouseKeyboard };
public enum Button { A, B, Up, Down, Left, Right, Plus, Minus, Home, One, Two, Z, C };
public enum Command { Jump, SelectObj, SelectUI }
public enum Axis { Horizontal, Vertical }

/// <summary>
/// Custom input manager that takes into account both Wii remotes and the mouse + keyboard
/// </summary>
public class InputManager : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    public InputMode mode;

    public static InputManager instance;
    public static Wiimote wiimote;
    private WiimoteGetButton getButton;

    [Header("Object References")]
    [HideInInspector] public WiimoteSetup wiimoteSetup;

    [Header("Pointer")]
    [SerializeField] private RectTransform pointer;
    [HideInInspector] public Camera cam;

    [Header("Shake Detection")]
    private bool _shaking;
    private Vector3 prevAccelValue;
    private float[] prevAccelAngles = new float[5];

    // -----------------------------------------------------------------------------------------------------------

    private void Awake() {
        getButton = GetComponent<WiimoteGetButton>();
    }

    private void Start() {
        cam = Camera.main; // TODO - update dynamically with scene
    }

    // -----------------------------------------------------------------------------------------------------------

    private void Update() {
        // Find wiimote
        if(!WiimoteManager.HasWiimote()) {
            // Remove cursor and exit if no wiimote found
            if(!wiimoteSetup.FindWiimote()) {
                pointer.anchorMax = new Vector2(-1f, -1f);
                pointer.anchorMin = new Vector2(-1f, -1f);
                return;
            }
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

        // Stop if using keyboard controls
        // (Can still use pointer for menus, but other functions aren't needed)
        if (mode == InputMode.MouseKeyboard)
            return;

        // ---

        // Shake
        if(Time.frameCount % 2 == 0) { // Every 2 frames
            CalculateShake();
        }
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Get Universal Inputs
    /// Returns the proper button input for the command, given the current input mode

    public bool GetCommandDown(Command command) {
        // UI happens irrelevant of mode
        if(command == Command.SelectUI)
            return GetWiimoteButtonDown(Button.A) || Input.GetMouseButtonDown(0);

        // Wiimote
        if(mode == InputMode.Wiimote) {
            if(command == Command.Jump)
                return GetWiimoteButtonDown(Button.A);
            else if(command == Command.SelectObj)
                return GetWiimoteButtonDown(Button.B);
            else
                return false;
        } else { // Mouse and keyboard
            if(command == Command.Jump)
                return Input.GetKeyDown(KeyCode.Space);
            else if(command == Command.SelectObj)
                return Input.GetMouseButtonDown(0);
            else
                return false;
        }
    }

    public bool GetCommandUp(Command command) {
        // UI happens irrelevant of mode
        if(command == Command.SelectUI)
            return GetWiimoteButtonUp(Button.A) || Input.GetMouseButtonUp(0);

        // Wiimote
        if(mode == InputMode.Wiimote) {
            if(command == Command.Jump)
                return GetWiimoteButtonUp(Button.A);
            else if(command == Command.SelectObj)
                return GetWiimoteButtonUp(Button.B);
            else
                return false;
        } else { // Mouse and keyboard
            if(command == Command.Jump)
                return Input.GetKeyUp(KeyCode.Space);
            else if(command == Command.SelectObj)
                return Input.GetMouseButtonUp(0);
            else
                return false;
        }
    }

    public bool GetCommand(Command command) {
        // UI happens irrelevant of mode
        if(command == Command.SelectUI)
            return GetWiimoteButton(Button.A) || Input.GetMouseButton(0);

        // Wiimote
        if(mode == InputMode.Wiimote) {
            if(command == Command.Jump)
                return GetWiimoteButton(Button.A);
            else if(command == Command.SelectObj)
                return GetWiimoteButton(Button.B);
            else
                return false;
        } else { // Mouse and keyboard
            if(command == Command.Jump)
                return Input.GetKey(KeyCode.Space);
            else if(command == Command.SelectObj)
                return Input.GetMouseButton(0);
            else
                return false;
        }
    }

    public float GetAxis(Axis axis) {
        if(mode == InputMode.Wiimote)
            return GetNunchuckAxis(axis);
        else
            return Input.GetAxis(axis.ToString());
    }

    #endregion

    // ------------------------

    #region Get Wiimote Buttons
    /// Equivalent of Input.GetButton and its variants, but for the wii remote

    public bool GetWiimoteButton(Button button) {
        return getButton.GetCorrespondingWiimoteButton(button);
    }

    public bool GetWiimoteButtonDown(Button button) {
        return getButton.buttonDown[button];
    }

    public bool GetWiimoteButtonUp(Button button) {
        return getButton.buttonUp[button];
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Pointer

    /// <summary>
    /// Returns a lerped/stabilized position for the wiimote pointer
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

    // -------------

    /// <summary>
    /// Returns the pointer or mouse's viewport position, depending on the input mode
    /// </summary>
    private Vector2 GetPointerViewportPos() {
        if(mode == InputMode.Wiimote)
            return pointer.anchorMin;
        else
            return GetMouseToViewportPosition();
    }

    /// <summary>
    /// Returns the direction vector of the pointer relative to the camera
    /// </summary>
    public Vector3 GetPointerDirectionVector() {
        Vector2 viewportPos = GetPointerViewportPos();
        Vector3 pointerPos = cam.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane));
        return (pointerPos - cam.transform.position).normalized;
    }

    // ---

    /// <summary>
    /// Returns the current object the pointer is aiming at
    /// </summary>
    /// <param name="mask">The LayerMask used for the detection raycast</param>
    /// <param name="maxDistance">Maximum distance to check</param>
    public GameObject SelectedObject(LayerMask mask, float maxDistance = 15f) {
        Vector3 direction = GetPointerDirectionVector();

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, direction, out hit, maxDistance, mask)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Returns whether or not the pointer is currently on screen
    /// </summary>
    public bool PointerOnScreen() {
        if(mode == InputMode.Wiimote)
            return pointer.anchorMax[0] != -1f;
        else {
            Vector2 mousePos = GetMouseToViewportPosition();
            return mousePos.x >= 0 && mousePos.x <= 1;
        }
    }

    #endregion

    // ------------------------

    #region Mouse-Specific

    /// <summary>
    /// Returns the mouse's position on the screen, from the range (0f, 1f) inclusive on both axes
    /// </summary>
    private Vector2 GetMouseToViewportPosition() {
        Vector2 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;
        return mousePos;
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
    public float GetNunchuckAxis(Axis axis) {
        if(wiimote == null)
            return 0;
        if(wiimote.current_ext != ExtensionController.NUNCHUCK) {
            Debug.LogError("Nunchuck not detected");
            return 0;
        }

        NunchuckData data = wiimote.Nunchuck;
        int value = 0;
        switch(axis) {
            case Axis.Horizontal:
                value = data.stick[0]; // General range is 35-228
                break;
            case Axis.Vertical:
                value = data.stick[1]; // General range is 27-220
                break;
            default:
                throw new System.ArgumentException("Invalid argument: " + axis + ", expected \"Horizontal\" or \"Vertical\"");
        }

        // Check if input mode not setup
        if(value == 0) {
            Debug.LogError("Attempting to read nunchuck data when input mode is not setup");
            wiimoteSetup.Setup();
            return 0f;
        }

        // Dead zone - Center is around 128, range of 16
        if(value > 112 && value < 144)
            return 0f;

        // Set horizontal to similar range as vertical
        if(axis == Axis.Horizontal)
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
