using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* References to on-scene objects. These will be used by various scripts. */
public class LocalReferences : MonoBehaviour {
    
    public static LocalReferences core;

    [Tooltip("Text area main container.")]
    public GameObject textAreaContainer;

    [Tooltip("Text Area scroll view")]
    public GameObject textAreaScrollA;

    [Tooltip("Main text area (where character dialogue appears).")]
    public GameObject textAreaA;

    [Tooltip("Character name label")]
    public GameObject nameObjectA;

    [Tooltip("Character avatar object.")]
    public GameObject characterAvatar;

    public GameObject clickAreaA;

    //Text Area B
    public GameObject textAreaScrollB;
    public GameObject textAreaB;
    public GameObject nameObjectB;
    public GameObject clickAreaB;

    [Tooltip("Choice area container.")]
    public GameObject choiceAreaContainer;

    [Tooltip("Choice area (ChoiceAreaContainer -> Scroll View -> View Port).")]
    public GameObject choiceArea;

    [Tooltip("The prefab to be spawned in the choice area.")]
    public GameObject choicePrefab;

    public GameObject toolContainer;

    [Tooltip("Settings window.")]
    public GameObject settingsContainer;
    public GameObject savesContainer;
    public GameObject loadContainer;
    
    public GameObject loadWindow;
    public GameObject savesWindow;
    public GameObject generalWindow;

    [Tooltip("Notifications window.")]
    public GameObject notificationsContainer;

    [Tooltip("Norifications window simple.")]
    public GameObject simpleNotificationsContainer;

    [Tooltip("Title screen object")]
    public GameObject titleScreenObject;

    [Tooltip("Input Container")]
    public GameObject inputContainer;
    
    [Tooltip("An object with an image component used to fade the screen in/out.")]
    public GameObject overlay;

    [Tooltip("Used to detect screen wide clicks.")]
    public GameObject clickCatcher;

    [Tooltip("Main menu object.")]
    public GameObject mainMenu;

    public Button continueButton;
    public Button saveWindowButton;

    [Tooltip("Quick menu object.")]
    public GameObject quickMenu;

    public GameObject backgroundObjectA;
    public GameObject backgroundObjectB;
    
    
    void Awake() {
        core = core == null ? this: core;
    }

}

// Code by Cination / Tsenkilidis Alexandros.