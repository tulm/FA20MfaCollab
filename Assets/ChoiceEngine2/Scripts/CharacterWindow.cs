using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Character window management */
public class CharacterWindow : MonoBehaviour {

    public static CharacterWindow core;

    void Awake() {
        core = core == null ? this : core;
    }
    
    public CanvasManager canvas;

    public Image avatar;
    public Text charName;
    public Text description;
    public Text age;
    public Text gender;
    public GameObject status;
    public GameObject affection;
    public Button statsButton;
    public GameObject statsWindow;
    public GameObject statsContainer;
    public GameObject statPrefab;

    //Updates the chatacter window with the infromation of the player character
    public void updatePlayerCharacterWindow() {
        var playerCharacter = Database.core.getPlayerCharacter();
        updateCharacterWindow(playerCharacter);
    }

    //Updating all info in the character window
    public void updateCharacterWindow(DBCharacter character) {

        //Setting character data
        avatar.sprite = Database.core.getSpriteById(character.avatarID);
        charName.text = character.isPlayer ? Database.core.playerName : Database.core.getContentByLanguage(character.name);
        age.text = "Age: " + Database.core.getContentByLanguage(character.age);
        gender.text = "Gender: " + Database.core.getContentByLanguage(character.gender);

        //Just in case, updating character status before assigning it.
        AffectionManager.core.updateStatus(character);

        //Status and affection.
        //If we are inspecting the player character, we want these disabled.
        if (!character.isPlayer) {
            status.GetComponent<Text>().text = "Relationship: " + character.affection.currentStatus;
            affection.GetComponent<Text>().text = "Affection: " + character.affection.currentValue.ToString();
            status.SetActive(true);
            affection.SetActive(true);
        } else {
            status.SetActive(false);
            affection.SetActive(false);
        }  

        description.text = Database.core.getContentByLanguage(character.description);

        //On button click, display character stats
        statsButton.onClick.RemoveAllListeners();
        statsButton.onClick.AddListener(delegate { genStats(character); statsWindow.SetActive(true); });

    }

    //Generating a list of all stats
    public void genStats(DBCharacter character) {

        foreach(Transform t in statsContainer.transform) {
            Destroy(t.gameObject);
        }

        foreach(Stat s in character.stats) {
            var stat = Instantiate(statPrefab, statsContainer.transform);
            stat.GetComponent<Text>().text = Database.core.getContentByLanguage(s.statName) + ": " + s.currentValue.ToString();
        }

    }

}

// Code by Cination / Tsenkilidis Alexandros.