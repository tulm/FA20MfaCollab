using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script manages background transitions. */
public class BackgroundManager : MonoBehaviour {

    //Creating reference to make the script accessible from other scripts
    public static BackgroundManager core;

    void Awake() {
        core = core == null ? this : core;
    }
    
    [Tooltip("Should fading be enabled?")]
    public bool fade;

    [Tooltip("Background fade speed. Suggested is 0.01.")]
    [Range(0.001f, 1)]
    public float fadeSpeed = 0.01f;

    [Tooltip("The ID of the background (in the Database) to appear first.")]
    public string startingBackground;

    //Indicates whether the background is currently fading
    [HideInInspector]
    public bool fading;
    
    //Two objects are used when fading
    [HideInInspector]
    public GameObject backgroundObjectA;

    [HideInInspector]
    public GameObject backgroundObjectB;

    [HideInInspector]
    public string currentBackground;

    [HideInInspector]
    public int changeCounter;

    //This functions initiates background transition
    public void ChangeBackground(string backgroundID) {

        var background = Database.core.getSpriteById(backgroundID);
        currentBackground = backgroundID;

        //If fade is active, initiate fade routine, otherwise simply set the background image
        if (fade && !SkipAuto.core.skip) {
            var c = changeCounter + 1;
            StartCoroutine(ChangeBackgroundEnum(background, c));
        } else {
            backgroundObjectA.GetComponent<Image>().sprite = background;
            backgroundObjectA.SetActive(true);
            backgroundObjectB.SetActive(false);
        }

    }

    //The enumerator responsible for fading background in/out
    public IEnumerator ChangeBackgroundEnum(Sprite background, int c) {
        
        var initCounter = changeCounter;

        //If the background is already changing, wait
        while (fading && initCounter == changeCounter) {
            yield return new WaitForEndOfFrame();
        }

        if (initCounter == changeCounter) {

            //Letting the script know that the background currently waiting
            fading = true;

            //Setting the foreground the same as the current background
            //This is done to mask further transitions in the background
            backgroundObjectB.GetComponent<Image>().sprite = backgroundObjectA.GetComponent<Image>().sprite;
            backgroundObjectB.SetActive(true);
            
            //Setting foreground alpha to 1 (making it visible)
            var colorA = backgroundObjectB.GetComponent<Image>().color;
            colorA.a = 1;
            backgroundObjectB.GetComponent<Image>().color = colorA;

            //Setting the background
            backgroundObjectA.GetComponent<Image>().sprite = background;

            //Transitioning from the foreground to the background again
            while (backgroundObjectB.GetComponent<Image>().color.a > 0 && initCounter == changeCounter) {
                var colorB = backgroundObjectB.GetComponent<Image>().color;
                colorB.a -= fadeSpeed;
                backgroundObjectB.GetComponent<Image>().color = colorB;
                yield return new WaitForEndOfFrame();
            }

            backgroundObjectB.SetActive(false);
            fading = false;

        }


    }

    //On start, get the background objects and change the initial background to the starting background
    void Start() {
        backgroundObjectA = LocalReferences.core.backgroundObjectA;
        backgroundObjectB = LocalReferences.core.backgroundObjectB;
    }

    public void initializeBackground() {
        //Setting starting background
        backgroundObjectA.GetComponent<Image>().sprite = Database.core.getSpriteById(startingBackground);
        backgroundObjectB.GetComponent<Image>().sprite = Database.core.getSpriteById(startingBackground);

        currentBackground = startingBackground;
    }

}

// Code by Cination / Tsenkilidis Alexandros.