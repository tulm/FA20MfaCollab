using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* A script used to load levels */
public class LevelLoader : MonoBehaviour {
    
    public static LevelLoader core;

    Coroutine loaderRoutine;

    public float screenFadeSpeed = 0.01f;

    //Called to load a level
    public void LoadLevel(int levelIndex, bool loadSave, GlobalSave gs) {
        if (loaderRoutine != null) {
            StopCoroutine(loaderRoutine);
        }

        loaderRoutine = StartCoroutine(loadLevelEnum(levelIndex, loadSave, gs));
    }

    //Enumerator that handles level loading
    IEnumerator loadLevelEnum (int levelIndex, bool loadSave, GlobalSave gs) {
        
        //Making sure that we are not trying to load the same level
        var overlay = LocalReferences.core.overlay;
        var c = overlay.GetComponent<Image>().color;
        c.a = 0;

        overlay.GetComponent<Image>().color = c;
        overlay.SetActive(true);

        //Fading in
        while(c.a < 1f) {
            c.a += screenFadeSpeed;
            overlay.GetComponent<Image>().color = c;
            yield return new WaitForEndOfFrame();
        }

        //Closing main menu
        MenuFunctions.core.closeMainMenu();

        var prevIndex = SceneManager.GetActiveScene().buildIndex;

        //Loading scene
        SceneManager.LoadScene(levelIndex);

        //Waiting for the scene to load
        while (SceneManager.GetActiveScene().buildIndex == prevIndex) {
            if (LevelManager.core != null && LevelManager.core.loaded) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        LevelManager.core.loaded = false;

        var nodeToLoad = "";

        //Loading save data
        if (loadSave) {
                    
            SaveManager.core.justLoaded = true;

            nodeToLoad = gs.currentNode;

            SoundManager.core.RestartAudio(gs.nowPlaying);

            Database.core.visitedNodes = gs.visitedNodes;

            Database.core.characters = gs.characters;
            Database.core.playerName = gs.playerName;
            LevelManager.core.currentNode = gs.currentNode;

            InventoryManager.core.inventoryItems = gs.inventoryItems;
            ResourceManager.core.inventoryResources = gs.inventoryResources;

            Database.core.gallery = gs.gallery;

            BackgroundManager.core.ChangeBackground(gs.background);
            
            SpawnManager.core.Respawn(gs.spawned);

            SkipAuto.core.loadVisited();

        } else {
            BackgroundManager.core.initializeBackground();
            nodeToLoad = LevelManager.core.currentNode;
        }

        //Displaying first/custom node.
        NodeManager.core.displayNode(nodeToLoad);

        //Fading out
        while(c.a > 0f) {
            c.a -= screenFadeSpeed;
            overlay.GetComponent<Image>().color = c;
            yield return new WaitForEndOfFrame();
        }

        overlay.SetActive(false);

    }

    void Awake() {
        core = core == null ? this : core;
    }
}

// Code by Cination / Tsenkilidis Alexandros.