using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour {

    [SerializeField] private Button button;
    [SerializeField] private string scene;

    private void Start() {
        GameManager.gm.loseScreen = gameObject;

        button.onClick.AddListener(() => {
            GameManager.gm.LoadScene(scene);
        });

        gameObject.SetActive(false);
    }

}