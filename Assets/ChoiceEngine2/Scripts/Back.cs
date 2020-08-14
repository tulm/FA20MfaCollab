using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is responsible for the back button, and it allows us to load the previous node. */
public class Back : MonoBehaviour {

    //Loading previous node if one exists
    public void loadPrevious() {
        
        SaveManager.core.StateLoad();

        var prev = LevelManager.core.getNodeById(LevelManager.core.currentNode).previousNode;
        var prevSave = LevelManager.core.getNodeById(LevelManager.core.currentNode).previousSave;

        if (prev != "#-1") {
            SaveManager.core.LocalLoadExtended(prevSave);
        }
        
    }    

}

// Code by Cination / Tsenkilidis Alexandros.