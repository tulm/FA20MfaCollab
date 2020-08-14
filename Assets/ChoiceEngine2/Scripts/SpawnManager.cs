using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script is responsible for character on-screen spawn. */
public class SpawnManager : MonoBehaviour {

    public static SpawnManager core;

    void Awake() {
        core = core == null ? this : core;
    }
    
    [Tooltip("A list of manually created spawn points.")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [HideInInspector]
    public List<string> terminate = new List<string>();

    [HideInInspector]
    public List<DisplayCharacter> spawned = new List<DisplayCharacter>();

    public void despawnAll() {
        foreach (SpawnPoint sp in spawnPoints) {
            despawn(sp.id);
        }
    }

    public void despawn(string spawnPointID) {

        terminate.Add(spawnPointID);
        var spawnPoint = getSpawnPointById(spawnPointID);
        var img = spawnPoint.GetComponent<Image>();

        if (img != null) {
            img.sprite = null;
        }

        foreach (Transform t in spawnPoint.transform) {
            Destroy(t.gameObject);
        }

        var toRemove = spawned.Find(x => x.spawnPointID == spawnPointID);

        if (toRemove != null) {
            spawned.Remove(toRemove);
        }

        spawnPoint.SetActive(false);

    }

    public void Respawn(List<DisplayCharacter> newSpawned) {

        despawnAll();

        foreach (DisplayCharacter dc in newSpawned) {
            spawn(dc);
        }
        
    }

    //Spawning a character
    public void spawn (DisplayCharacter dc) {

        var spawnPointID = dc.spawnPointID;
        var modelID = dc.modelID;
        var spriteID = dc.spriteID;
        var loopAnimation = dc.loopAnimation;
        var frames = dc.animationFrames;

        var spawnPoint = getSpawnPointById(spawnPointID);

        var rem = spawned.Find(x => x.spawnPointID == spawnPointID);
        if (rem != null) {
           spawned.Remove(rem); 
        }
        
        if (!spawned.Contains(dc)) {
            spawned.Add(dc);
        }

        var type = dc.spawnType;

        foreach(Transform t in spawnPoint.transform) {
            Destroy(t.gameObject);
        }

        spawnPoint.SetActive(true);

        if (type == DisplayCharacter.spawnTypeEnum.Sprite) {

            var characterSprite = Database.core.getSpriteById(spriteID);

            //If no image component exist, one will be added
            if (spawnPoint.GetComponent<Image>() == null) {
                spawnPoint.AddComponent<Image>();
                
                Debug.Log(@"No image component found attached to your spawnpoint. One was added automatically, 
                however it is recommended that you add and configure am image component beforehand.");
            }

            spawnPoint.GetComponent<Image>().preserveAspect = true;
            spawnPoint.GetComponent<Image>().sprite = characterSprite;
            
        } else if (type == DisplayCharacter.spawnTypeEnum.Model) {

            var characterModel = Database.core.getPrefabById(modelID);
            var spawned = Instantiate(characterModel, spawnPoint.transform);

        } else {

            //If no image component exist, one will be added
            if (spawnPoint.GetComponent<Image>() == null) {
                spawnPoint.AddComponent<Image>();
                
                Debug.Log(@"No image component found attached to your spawnpoint. One was added automatically, 
                however it is recommended that you add and configure am image component beforehand.");
            }

            StartCoroutine(animationEnumerator(frames, spawnPoint, spawnPointID, loopAnimation));

        }
        
    }

    //Sprite animations
    IEnumerator animationEnumerator(List<Frame> frames, GameObject spawnPoint, string spawnPointID, bool loop) {

        spawnPoint.GetComponent<Image>().preserveAspect = true;

        while (true) {

            foreach (Frame f in frames) {
                var sp = Database.core.getSpriteById(f.spriteID);
                spawnPoint.GetComponent<Image>().sprite = sp;
                
                yield return new WaitForSeconds(f.delay);
            }

            if (!loop || terminate.Contains(spawnPointID)) {
                break;
            }

            yield return new WaitForEndOfFrame();

        }

    }

    public GameObject getSpawnPointById(string id) {
        var spawn = spawnPoints.Find(x => x.id == id);
        if (spawn == null) {
            Debug.Log("There is no spawn point with ID " + id + ". Spawn points can be created via the spawn manager script, under the canvas object.");
        }
        return spawn != null ? spawn.spawnPoint : null;
    }

}

//A class containing info about individual spawn points
[System.Serializable]
public class SpawnPoint {
    public string id;
    public GameObject spawnPoint;

}

// Code by Cination / Tsenkilidis Alexandros.