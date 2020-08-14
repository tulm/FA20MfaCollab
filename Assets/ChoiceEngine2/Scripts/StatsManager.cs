using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Character stat management */

public class StatsManager : MonoBehaviour {
    public static StatsManager core;

    public void increaseStat(DBCharacter character, string statID, float v) {

        Stat stat = Database.core.getStatById(character, statID);
        stat.currentValue = stat.currentValue + v > stat.maxValue ? stat.maxValue : stat.currentValue + v;
    }

    public void descreaseStat(DBCharacter character, string statID, float v) {
        Stat stat = Database.core.getStatById(character, statID);
        stat.currentValue = stat.currentValue - v < 0 ? 0 : stat.currentValue - v;
    }

    public void setStat(DBCharacter character, string statID, float v) {
        Stat stat = Database.core.getStatById(character, statID);
        stat.currentValue = v >= 0 && v <= stat.maxValue ? v : stat.currentValue;
    }

    public bool checkStat(DBCharacter character, string statID, float requiredValue) {
        Stat stat = Database.core.getStatById(character, statID);
        return stat.currentValue >= requiredValue;
    }
    
    void Awake() {
        core = core == null ? this : core;
    }
}

// Code by Cination / Tsenkilidis Alexandros.