using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The database is a simple scirpt that stores references to images and audio used throughout the project. */
public class Database : MonoBehaviour {
    
    public enum databaseMenuEnum { Characters, Images, Audio, Prefabs, Languages, Gallery, Quests, Resources, Items, Misc }
    public databaseMenuEnum databaseMenu;

    //Player's name
    public string playerName;

    //Reference
    public static Database core;

    [Tooltip("A list of all characters.")]
    public List<DBCharacter> characters = new List<DBCharacter>();

    [Tooltip("A list of all images.")]
    public List<DBImage> images = new List<DBImage>();

    [Tooltip("A list of all audio.")]
    public List<DBAudio> audios = new List<DBAudio>();

    [Tooltip("A list of all prefabs.")]
    public List<DBPrefab> prefabs = new List<DBPrefab>();

    [Tooltip("A list of all available languages.")]
    public List<string> languages = new List<string>();

    [Tooltip("A list of gallery images and their states")]
    public List<GalleryImage> gallery = new List<GalleryImage>();

    [Tooltip("A list of all quests.")]
    public List<Quest> quests = new List<Quest>();

    [Tooltip("A list of all in-game resources.")]
    public List<Resource> resources = new List<Resource>();

    [Tooltip("A list of all in-game items.")]
    public List<Item> items = new List<Item>();

    [Tooltip("Placeholder Image.")]
    public Sprite placeholder;

    [Tooltip("Image locked overlay.")]
    public Sprite locked;

    //A list of all visited nodes. This list is used to skip nodes and go back to past nodes.
    [HideInInspector]
    public List<VisitedNode> visitedNodes = new List<VisitedNode>();

    [HideInInspector]
    public List<VisitedNode> visitedNodesGlobal = new List<VisitedNode>();

    public Stat getStatById(DBCharacter character, string statID) {
        return character.stats.Find(x => x.statID == statID);
    }

    public DBCharacter getCharacterById(string id) {
        return characters.Find(x => x.id == id);
    } 

    public Sprite getSpriteById(string id) {
        var z = images.Find(x => x.id == id);
        return z != null ? z.image : placeholder;
    } 

    public GameObject getPrefabById(string id) {
        return prefabs.Find(x => x.id == id).prefab;
    }

    public AudioClip getAudioById(string id) {
        return audios.Find(x => x.id == id).audioclip;
    }

    public DBCharacter getPlayerCharacter() {
        return characters.Find(x => x.isPlayer == true);
    }

    public Item getItemById(string id) {
        return items.Find(x => x.itemID == id);
    }

    public Resource getResourceById(string id) {
        return resources.Find(x => x.resourceID == id);
    }

    public string getContentByLanguage(List<String> list) {

        try {
            if (list.Count > 0 && list[0].language == "") {
                return list[0].content;;
            } else {
                var z = list.Find(x => x.language == languages[Settings.core.settings.language]).content;
                return z;  
            }
            
        } catch {
            //Debug.Log("It seems that you've made a mistake. Please ensure that the language ID of all of your nodes is correct.");
            return "";
        }
        
    }
    
    public String getObjectByLanguage(List<String> list) {
        return list.Find(x => x.language == languages[Settings.core.settings.language]);
    }

    public Quest getQuestById(string id) {
        return quests.Find(x => x.questID == id);
    }

    public GalleryImage getGalleryImageObjectById(string id) {
        return gallery.Find(x => x.galleryImageID == id);
    }

    void Awake() {
        //Initializig reference
        core = core == null ? this : core;

    }

}

[System.Serializable]
public class VisitedNode {

    public int levelIndex;
    public string nodeID;
    public string name;
    public string content;
    
}

[System.Serializable]
public class DBCharacter {

    //Base info
    public string id;
    public List<String> name = new List<String>();

    [Tooltip("Is this the player character?")]
    public bool isPlayer;

    [Tooltip("Should the character be displayed in the characters window?")]
    public bool showInCharactersList;

    public List<String> age = new List<String>();
    public List<String> gender = new List<String>();

    [Tooltip("The id of the character avatar in the database.")]
    public string avatarID;
    
    public List<String> description = new List<String>();

    //Affection
    [Tooltip("Affection config")]
    public Affection affection = new Affection();

    //Stats
    [Tooltip("The stats of the character (e.g Agility, Strength etc).")]
    public List<Stat> stats = new List<Stat>();

}

[System.Serializable]
public class Stat {
    public string statID;
    public List<String> statName = new List<String>();
    public float currentValue;
    public float maxValue;
}

[System.Serializable]
public class Affection {

    [Tooltip("The affection level of the character towards the player at any given point in time.")]
    public float currentValue;

    [Tooltip("The maximum possible affection level.")]
    public float maxValue;

    [HideInInspector]
    public string currentStatus = "No Status";

    [Tooltip("Affection labels that correlate to affection levels.")]
    public List<AffectionStatus> affectionLevels = new List<AffectionStatus>();

}

[System.Serializable]
public class AffectionStatus {

    [Tooltip("Once the required affection is reached, this will be the status of the character (i.e. their relationship with you).")]
    public List<String> status = new List<String>();

    [Tooltip("The affection value required for reaching this status.")]
    public float requiredAffection;
}


[System.Serializable]
public class DBPrefab {
    [Tooltip("The id used to retrieve the prefab. The id is a string.")]
    public string id;

    [Tooltip("The prefab retrieved by using the id.")]
    public GameObject prefab;
}

[System.Serializable]
public class DBImage {

    [Tooltip("The id used to retrieve the image. The id is a string.")]
    public string id;

    [Tooltip("The image to be retrieved using the id.")]
    public Sprite image;

}

[System.Serializable]
public class DBAudio {

    [Tooltip("The id used to retrieve the audio. The id is a string.")]
    public string id;

    [Tooltip("The audio to be retrieved using the id.")]
    public AudioClip audioclip;

}

[System.Serializable]
public class String {

    [Tooltip("The name of the language in the database.")]
    public string language;

    [TextArea]
    public string content;

}

[System.Serializable]
public class GalleryImage {
    
    [Tooltip("The ID of the image in the database.")]
    public string galleryImageID;

    [Tooltip("Is the image locked?")]
    public bool locked;
}

[System.Serializable]
public class Quest {

    [Tooltip("Unique quest ID")]
    public string questID;

    [Tooltip("Has the quest been assigned to the player?")]
    public bool assigned;

    public enum questStateEnum {Completed, Failed}
    
    [Tooltip("The state of the quest")]
    public questStateEnum questState;

    [Tooltip("Quest name.")]
    public List<String> questName = new List<String>();

    [Tooltip("Quest description.")]
    public List<String> questDescription = new List<String>();

    [Tooltip("The conditions to be fulfilled for the quest to be completed.")]
    public EntryConidtions completionConditions = new EntryConidtions();
}

[System.Serializable]
public class Item {

    [Tooltip("A unique item ID.")]
    public string itemID;

    [Tooltip("The name of the item.")]
    public List<String> itemName = new List<String>();

    [Tooltip("The id of the item's icon in the database (image section).")]
    public string itemIconID;

    [Tooltip("The descrition of the item.")]
    public List<String> description = new List<String>();

    [Tooltip("The quantity of an item at any given point in time.")]
    public float currentQuantity;

    [Tooltip("The maximum quantity of the item that the player can have.")]
    public float maxQuantity;

    [Tooltip("The function from UserFunctions.cs that will be triggered once the item is used.")]
    public string useFunction;

}


[System.Serializable]
public class Resource {

    [Tooltip("A unique resource ID.")]
    public string resourceID;

    [Tooltip("The name of the item.")]
    public List<String> resourceName = new List<String>();

    [Tooltip("The id of the resource's icon in the database (image section).")]
    public string resourceIconID;

    [Tooltip("The descrition of the resource.")]
    public List<String> description = new List<String>();

    [Tooltip("The quantity of a resource at any given point in time.")]
    public float currentQuantity;

    [Tooltip("The maximum quantity of the resource that the player can have.")]
    public float maxQuantity;

    [Tooltip("The function from UserFunctions.cs that will be triggered once the resource is used.")]
    public string useFunction;

}

// Code by Cination / Tsenkilidis Alexandros.