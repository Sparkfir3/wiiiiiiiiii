using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour {

    [SerializeField] private Button restartButton, mainMenuButton;
    [SerializeField] private string scene;

    private void Start() {
        GameManager.gm.loseScreen = gameObject;

        restartButton.onClick.AddListener(() => GameManager.gm.LoadScene(scene));
        mainMenuButton.onClick.AddListener(() => GameManager.gm.LoadScene("Main Menu"));

        gameObject.SetActive(false);
    }

}