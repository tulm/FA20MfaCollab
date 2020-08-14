using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/* This is the only script unique to each level. All level unique settings and content should be here. */
public class LevelManager : MonoBehaviour {

    public static LevelManager core;

    public List<Node> nodes = new List<Node>();
    public string startingNode;

    [HideInInspector]
    public int ignoreMe = 0;
    
    [HideInInspector]
    public string currentNode;

    [HideInInspector]
    public bool loaded;
    
    void Start() {

        //Should we load a custom starting node?
        var csn = NodeManager.core.customStartingNode;
        currentNode = csn != "#-1" ? csn : startingNode;

        //Resetting custom node.
        NodeManager.core.customStartingNode = "#-1";

        //Letting engine know that the scene has been loaded
        loaded = true;
        
    }

    void Awake() {
        core = core == null ? this : core;
    }

    //Returns a node with the same id as provided
    public Node getNodeById(string id) {
        return nodes.Find(x => x.id == id);
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

    
}

// Code by Cination / Tsenkilidis Alexandros.