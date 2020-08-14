using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* Used to save and load data. Objects must be serializable. */
public class SaveManager : MonoBehaviour {

    public static SaveManager core;

    [HideInInspector]
    public bool justLoaded;

    public void qSave() {
        GlobalSave(true, "quickSave", "");
    }

    public void qLoad() {
        GlobalLoad("quickSave");
    }

    //Saving as an object
    public GlobalSave LocalSave() {

        GlobalSave gs = new GlobalSave();
        gs.currentLevel = SceneManager.GetActiveScene().buildIndex;
        gs.nowPlaying = SoundManager.core.nowPlaying;
        gs.visitedNodes = Database.core.visitedNodes;
        gs.currentNode = LevelManager.core.currentNode;
        gs.characters = Database.core.characters;
        gs.playerName = Database.core.playerName;
        gs.gallery = Database.core.gallery;
        gs.background = BackgroundManager.core.currentBackground;
        gs.inventoryItems = InventoryManager.core.inventoryItems;
        gs.inventoryResources = ResourceManager.core.inventoryResources;
        gs.spawned = SpawnManager.core.spawned;

        SkipAuto.core.saveVisited();

        //We save and write data back to create a simple deep copy.
        JsonSave(gs, "temp1", ".temp");

        GlobalSave gs2 = new GlobalSave();
        JsonLoad(gs2, "temp1", ".temp");

        return gs2;

    }

    //Loading from object with the ability to load scenes
    public void LocalLoadExtended(GlobalSave gs) {

        if (gs.currentLevel != SceneManager.GetActiveScene().buildIndex) {
            LevelLoader.core.LoadLevel(gs.currentLevel, true, gs); 
        } else {
            LocalLoad(gs);
        }

    }

    //Loading from an object
    public void LocalLoad(GlobalSave gs) {

        justLoaded = true;

        MenuFunctions.core.closeMainMenu();

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

        NodeManager.core.displayNode(gs.currentNode);

        SkipAuto.core.loadVisited();

    }

    //Saving as a file
    public void GlobalSave(bool overwrite, string fileToOverwrite, string name) {

        if (LevelManager.core != null) {

            var postfix = 0;
            var filename = overwrite ? fileToOverwrite : name;

            name = name == "" ? "save" : name;

            //If we are not overwriting the file, it is important to check for existing files.
            if (!overwrite) {

                while (File.Exists (Application.persistentDataPath + "/" + name + "_" + postfix.ToString() + ".save")) {
                    postfix += 1;
                }

                name = name + "_" + postfix.ToString();
                filename = name;

            }

            //Saving the name of the latest save into player prefs
            //You can retriew this by using var latest = PlayerPrefs.GetString("Latest");
            PlayerPrefs.SetString("Latest", filename);

            GlobalSave gs = new GlobalSave();
            gs = LocalSave();

            JsonSave(gs, filename, ".save");

            LocalReferences.core.settingsContainer.SetActive(false);

            //Taking a screenshot and saving it. It will be used as the save thumbnail
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/" + filename + ".png");

        }

    }

    //Loading from a file
    public void GlobalLoad(string filename) {

        GlobalSave gs = new GlobalSave();
        JsonLoad(gs, filename, ".save");
        LocalLoadExtended(gs);

    }

    public void StateSave() {

        var stateSaveContainer = new StateSaveContainer();
        stateSaveContainer.levelIndex = SceneManager.GetActiveScene().buildIndex;

        foreach (Node n in LevelManager.core.nodes) {
            var state = new StateSave();
            state.nodeID = n.id;
            state.previousSave = n.previousSave;
            state.previousNode = n.previousNode;

            stateSaveContainer.states.Add(state);
        }

        JsonSave(stateSaveContainer, "/states" + stateSaveContainer.levelIndex.ToString(), ".states");

    }

    public void StateLoad() {

        var currentLevel = SceneManager.GetActiveScene().buildIndex;
        var fileName = "/states" + currentLevel.ToString() + ".states";

        if (File.Exists (Application.persistentDataPath + fileName)) {
            var statesConatiner = new StateSaveContainer();

            JsonLoad(statesConatiner, "/states" + currentLevel, ".states");
            
            var states = statesConatiner.states;

            foreach (StateSave s in states) {
                var node = LevelManager.core.getNodeById(s.nodeID);
                node.previousNode = s.previousNode;
                node.previousSave = s.previousSave;
            }

        }
        
    }

    //Saving as Json
    public void JsonSave (object toSave, string filename, string format) {
        string json = JsonUtility.ToJson(toSave, true);
        System.IO.File.WriteAllText (Application.persistentDataPath + "/" + filename + format, json);
    }

    //Loading from Json
    public void JsonLoad (object toLoad, string filename, string format) {

        if (File.Exists (Application.persistentDataPath + "/" + filename + format)) {
            string json = System.IO.File.ReadAllText (Application.persistentDataPath + "/" + filename + format);
            JsonUtility.FromJsonOverwrite(json, toLoad);
        }
        
    }

    //Saving the nodes list
    public void exportNodes(string filename) {
        //This function is used outside of runtime. This means that we have to manually find the object.
        var r = FindObjectOfType<LevelManager>();
        JsonSave(r, filename, ".json");
        Debug.Log("Nodes exported to " + Application.persistentDataPath + "/" + filename + ".json");
    }

    //Loading nodesList
    public void importNodes(string filename) {
        var r = FindObjectOfType<LevelManager>();
        JsonLoad(r, filename, ".json");
        Debug.Log("Nodes imported from " + Application.persistentDataPath + "/" + filename + ".json");
    }

    void Awake() {
        core = core == null ? this : core;
    }

}

[System.Serializable]
public class GlobalSave {

    public int currentLevel;

    public List<AudioInfo> nowPlaying = new List<AudioInfo>();

    public string background;

    public List<VisitedNode> visitedNodes = new List<VisitedNode>();

    public string currentNode;

    public string playerName;
    
    public List<DBCharacter> characters = new List<DBCharacter>();

    public List<GalleryImage> gallery = new List<GalleryImage>();

    public List<Item> inventoryItems = new List<Item>();

    public List<Resource> inventoryResources = new List<Resource>();

    public List<DisplayCharacter> spawned = new List<DisplayCharacter>();

}

[System.Serializable]
public class StateSaveContainer {
    public int levelIndex;
    public List<StateSave> states = new List<StateSave>();
}

[System.Serializable]
public class StateSave {
    public string nodeID;
    public GlobalSave previousSave;
    public string previousNode;
}

// Code by Cination / Tsenkilidis Alexandros.