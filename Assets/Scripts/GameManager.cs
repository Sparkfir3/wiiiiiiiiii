using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    public static GameManager gm;
    
    [Header("Object References")]
    [SerializeField] private GameObject cursorCanvas;
    [SerializeField] private WiimoteSetup wiimoteSetup;
    private InputManager inputs;

    [Header("Events")]
    public UnityEvent OnLoadScene;

    private void Awake() {
        // Singleton initialization
        if(gm == null) {
            gm = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // Other
        DontDestroyOnLoad(cursorCanvas);
        DontDestroyOnLoad(wiimoteSetup.gameObject);
        cursorCanvas.SetActive(true);

        // Input Manager
        inputs = GetComponent<InputManager>();
        InputManager.instance = inputs;
        InputManager.instance.wiimoteSetup = wiimoteSetup;
    }

    private void Start() {
        OnLoadScene.Invoke();
    }

}
