using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Character affection management */
public class AffectionManager : MonoBehaviour {
    public static AffectionManager core;

    //Increasing affection
    public void increaseAffection(DBCharacter character, float v) {

        character.affection.currentValue = character.affection.currentValue + v > character.affection.maxValue ? character.affection.maxValue : character.affection.currentValue + v;
        updateStatus(character);
    }

    //Descreasing affection
    public void descreaseAffection(DBCharacter character, float v) {
        character.affection.currentValue = character.affection.currentValue - v < 0 ? 0 : character.affection.currentValue - v;
        updateStatus(character);
    }

    //Setting affection to specific value
    public void setAffection(DBCharacter character, float v) {
        character.affection.currentValue = v >= 0 && v <= character.affection.maxValue ? v : character.affection.currentValue;
        updateStatus(character);
    }

    //Checking if the affections value if higher than some threshold
    public bool checkAffection(DBCharacter character, float requiredAffection) {

        if (character.affection.currentValue >= requiredAffection) {
            return true;
        }
        
        return false;
    }

    //Used to update status after chaining character affection
    public void updateStatus(DBCharacter character) {

        var status = "";

        foreach (AffectionStatus affection in character.affection.affectionLevels) {
            if (character.affection.currentValue >= affection.requiredAffection) {
                status = Database.core.getContentByLanguage(affection.status);
            }
        }

        character.affection.currentStatus = status;
    }

    void Awake() {
        core = core == null ? this : core;
    }
    
}

// Code by Cination / Tsenkilidis Alexandros.