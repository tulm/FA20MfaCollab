using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used to make keep objects on the scene when loading levels. */
public class DoNotDestroyOnLoad : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    
}
