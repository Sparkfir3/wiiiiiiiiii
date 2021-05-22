using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    public void StartGame() {
        GameManager.gm.LoadScene("Test Scene");
    }

    public void QuitGame() {
#if UNITY_EDITOR
        Debug.Log("Attempting to quit game, but cannot proceed in editor.");
#endif
        Application.Quit();
    }

}
