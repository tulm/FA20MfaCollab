using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersWindow : MonoBehaviour {
    
    public GameObject charactersContent;
    public GameObject characterPrefab;
    public GameObject characterWindow;

    //Displaying all the available characters in the characters window
    public void updateCharactersWindow() {

        //Destroying all current object
        foreach (Transform t in charactersContent.transform) {
            Destroy(t.gameObject);
        }

        var characters = Database.core.characters;

        //Generating characters list
        foreach (DBCharacter c in characters) {
            if (!c.isPlayer && c.showInCharactersList) {
                var character = Instantiate(characterPrefab, charactersContent.transform);
                character.transform.GetChild(0).gameObject.GetComponent<Text>().text = Database.core.getContentByLanguage(c.name);
                character.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Database.core.getSpriteById(c.avatarID);
                
                //On button click, displaying character window, managed by CharacterWindow.cs
                var b = character.GetComponent<Button>();
                b.onClick.AddListener(delegate { CharacterWindow.core.updateCharacterWindow(c); characterWindow.SetActive(true); });
            }
            
        }

    }

}

// Code by Cination / Tsenkilidis Alexandros.