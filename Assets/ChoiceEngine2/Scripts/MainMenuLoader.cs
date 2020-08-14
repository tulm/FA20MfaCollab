using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour {

    [HideInInspector]
    public int mainMenuSceneIndex;

    public void LoadMainMenu() {
        
        foreach (GameObject g in FindObjectsOfType<GameObject>()) {
            if (g.GetComponent<DoNotDestroyOnLoad>() != null) {
                Destroy(g);
            }
        }

        SceneManager.LoadScene(mainMenuSceneIndex);
        Destroy(gameObject);

    }


}
