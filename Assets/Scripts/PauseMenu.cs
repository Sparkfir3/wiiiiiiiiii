using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    private void Start() {
        GameManager.gm.PauseMenu = gameObject;
        gameObject.SetActive(false);
    }

    public void Resume() {
        GameManager.gm.Paused = false;
    }

    public void MainMenu() {
        GameManager.gm.LoadScene("Main Menu");
    }

}