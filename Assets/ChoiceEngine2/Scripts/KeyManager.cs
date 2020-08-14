using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script manages keybindings. Keys can be associated with functions from UserFunctions.cs, and call said functions on click. */
public class KeyManager : MonoBehaviour {
    
    [Tooltip("The list of all keybindings.")]
    public List<Keybind> keybindings = new List<Keybind>();

    //Simply monitoring for key presses.
    void Update() {

        foreach (Keybind k in keybindings) {

            if (Input.GetKeyDown(k.key)) {

                UserFunctions.core.Invoke(k.targetFunction, k.delay);

            }

        }

    }

}

[System.Serializable]
public class Keybind {
    [Tooltip("The key to which the function will be bound.")]
    public string key;

    [Tooltip("The function from UserFunctions.cs to be invoked.")]
    public string targetFunction;

    [Tooltip("The delay before invoking the function.")]
    public float delay;
}

// Code by Cination / Tsenkilidis Alexandros.