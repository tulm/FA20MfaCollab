using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Used to fade screen in and out. */
public class FadeManager : MonoBehaviour {

    public static FadeManager core;

    void Awake() {
        core = core == null ? this : core;
    }

    GameObject fadeObject;
    Coroutine fadeInRoutine;
    Coroutine fadeOutRoutine;

    //Indicates that the overlay is currently fading
    bool fading = false;

    //Fade speed
    public float fadeSpeed = 0.01f;

    //Getting overlay object on start
    void Start() {
        fadeObject = LocalReferences.core.overlay;
    }

    //Called to fade screen in
    public void fadeIn() {
        if (fadeInRoutine != null) {
            StopCoroutine (fadeInRoutine);
        }

        fadeInRoutine = StartCoroutine (fadeInEnum());
    }

    //Enum to used fade screen in
    IEnumerator fadeInEnum() {

        //If the screen is currently fading, wait for it to be done
        while (fading) {
            yield return new WaitForEndOfFrame();
        }

        //Telling the script that the overlay is currently fading
        fading = true;

        //Set alpha to 0
        var colorA = fadeObject.GetComponent<Image>().color;
        colorA.a = 0;
        fadeObject.GetComponent<Image>().color = colorA;

        //Activate fade object
        fadeObject.SetActive(true);

        //Fading alpha in
        while (fadeObject.GetComponent<Image>().color.a < 1) {
            var colorB = fadeObject.GetComponent<Image>().color;
            colorB.a += fadeSpeed;
            fadeObject.GetComponent<Image>().color = colorB;
            yield return new WaitForEndOfFrame();
        }

        //Indicating that the fading is done
        fading = false;

    }

    //Fading overlay out
    public void fadeOut() {

        if (fadeOutRoutine != null) {
            StopCoroutine (fadeOutRoutine);
        }

        fadeOutRoutine = StartCoroutine (fadeOutEnum());

    }

    //Enum responsible for fading screen out
    IEnumerator fadeOutEnum() {

        //If the overlay is already fading, wait for it to be done
        while (fading) {
            yield return new WaitForEndOfFrame();
        }

        //Indicating that overlay is currently fading
        fading = true;

        //Setting alpha to 1
        var colorA = fadeObject.GetComponent<Image>().color;
        colorA.a = 1;
        fadeObject.GetComponent<Image>().color = colorA;

        //Enabling overlay object
        fadeObject.SetActive(true);

        //Descreading alpha to fade out
        while (fadeObject.GetComponent<Image>().color.a > 0) {
            var colorB = fadeObject.GetComponent<Image>().color;
            colorB.a -= fadeSpeed;
            fadeObject.GetComponent<Image>().color = colorB;
            yield return new WaitForEndOfFrame();
        }

        fadeObject.SetActive(false);
        fading = false;

    }

}

// Code by Cination / Tsenkilidis Alexandros.