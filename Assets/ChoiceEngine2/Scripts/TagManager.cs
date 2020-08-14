using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script handles tags. Currently this script only handles tags in nodes' text, however, it can be modified it to handle other text as well. */

public class TagManager : MonoBehaviour {

    public static TagManager core;
    
    public List<Tag> tags = new List<Tag>();

    //This function should be called to replace any tags in a text field before it is displayed.
    public string replaceTags(string originalText) {

        var updatedText = originalText;

        foreach (Tag t in tags) {

            var toBeDisplayed = "";

            switch (t.tagType) {
                case Tag.tagTypeEnum.CharacterName:
                    toBeDisplayed = Database.core.getContentByLanguage(Database.core.getCharacterById(t.characterID).name);
                    break;
                case Tag.tagTypeEnum.PlayerName:
                    toBeDisplayed = Database.core.playerName;
                    break;
                default:
                    toBeDisplayed = Database.core.getContentByLanguage(t.customText);
                    break;
            }

            var failSafe = 0;
            while (updatedText.Contains(t.tag) && failSafe < 100) {
                updatedText = updatedText.Substring(0, updatedText.IndexOf(t.tag)) + toBeDisplayed + updatedText.Substring(updatedText.IndexOf(t.tag) + t.tag.Length, updatedText.Length - (updatedText.IndexOf(t.tag) + t.tag.Length));
                failSafe += 1;
                if (failSafe >= 100) {
                    Debug.Log("It seems that there has been an issue with the TagManager. If you see the message please report this to me directly.");
                }
            }

        }

        return updatedText;
    }

    void Awake() {
        core = core == null ? this : core;
    }

}

[System.Serializable]
public class Tag {
    public string tag;
    public enum tagTypeEnum { PlayerName, CharacterName, customText }

    [Tooltip("Determines what should be displayed by the tag.")]
    public tagTypeEnum tagType;

    [Tooltip("If the \"charcter name\" option has been selected, the tag will be replaced by the name of the character with the following id.")]
    public string characterID;

    [Tooltip("If the \"custom text\" option has been selected, the tag will be replaced with the following custom text (depending on the language).")]
    public List<String> customText = new List<String>();

    [Tooltip("If the respective option is selected as the tag type, the valued returned by a custom function in UserFunctions.cs (must be a string) will be used.")]
    public string customFunction;
}

// Code by Cination / Tsenkilidis Alexandros.