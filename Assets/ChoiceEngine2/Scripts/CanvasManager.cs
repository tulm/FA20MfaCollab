using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Functions to be used by local cavas elements without the need to reference external or runtime scripts. */
public class CanvasManager : MonoBehaviour {

    public static CanvasManager core;

    //Toggle the object on/off
    public void toggleObject(GameObject obj) {
        obj.SetActive(!obj.activeSelf);
    }

    //Enable objects
    public void enableObject(GameObject obj) {
        obj.SetActive(true);
    }

    //Disable object
    public void disableObject(GameObject obj) {
        obj.SetActive(false);
    }

    void Awake() {
        core = core == null ? this : core;
        DontDestroyOnLoad(this.gameObject);
    }

}

// Code by Cination / Tsenkilidis Alexandros.