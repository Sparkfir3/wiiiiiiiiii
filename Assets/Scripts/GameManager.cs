using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    public static GameManager gm;
    
    [Header("Cursor Management")]
    [SerializeField] private GameObject cursorCanvas;
    [SerializeField] private WiimoteSetup wiimoteSetup;
    private InputManager inputs;

    [Header("Game Control")]
    public GameState currentState;
    public GameObject loseScreen;

    [Header("Pause Control")]
    [SerializeField] private GameObject pauseMenu;
    private bool _paused;

    [Header("Events")]
    public UnityEvent OnLoadScene;

    // -----------------------------------------------------------------------------------------------------------

    #region Variables

    public bool Paused {
        get {
            return _paused;
        }
        set {
            if(_paused == value || !CanPause())
                return;

            _paused = value;
            if(_paused)
                PauseGame();
            else
                UnpauseGame();
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Initialization

    private void Awake() {
        // Singleton initialization
        if(gm == null) {
            gm = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(cursorCanvas);
            Destroy(wiimoteSetup.gameObject);
            Destroy(gameObject);
            return;
        }

        // Cursor management
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

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    private void Update() {
        if(InputManager.instance.GetCommandDown(Command.Pause))
            Paused = !Paused;
    }

    // -----------------------------------------------------------------------------------------------------------

    #region Game Control

    public void LoseGame() {
        Time.timeScale = 0;
        currentState = GameState.Lose;
        loseScreen.SetActive(true);
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Pause Control

    private void PauseGame() {
        Time.timeScale = 0;

        if(pauseMenu)
            pauseMenu.SetActive(true);
    }

    public void UnpauseGame() {
        if(pauseMenu)
            pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }

    private bool CanPause() {
        return currentState == GameState.Gameplay || currentState == GameState.Paused;
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Scene Management

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
        OnLoadScene.Invoke();

        currentState = GameState.Gameplay;
        Paused = false;
        Time.timeScale = 1;
    }

    #endregion

}
