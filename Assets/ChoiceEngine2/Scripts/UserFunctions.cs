using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* You can add your own custom functions here. */
public class UserFunctions : MonoBehaviour {
    
    public static UserFunctions core;

    public void toggleUI() {
        var n = LevelManager.core.getNodeById(LevelManager.core.currentNode);
        if (!n.hidden) {
            LocalReferences.core.textAreaContainer.SetActive(!LocalReferences.core.textAreaContainer.activeSelf);
        }
    }

    public void toggleQuickMenu() {
        var menu = LocalReferences.core.quickMenu;
        menu.SetActive(!menu.activeSelf);
    }

    public void test() {
        Debug.Log("Test!");
    }

    public void changeAffectionA() {
        var c = Database.core.getCharacterById("CharacterIDGoesHere");
        AffectionManager.core.increaseAffection(c, 50);
    }

    void Awake() {
        core = core == null ? this : null;
    }
    
}

// Code by Cination / Tsenkilidis Alexandros.