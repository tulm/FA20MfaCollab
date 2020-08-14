using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script handles the image gallery */
public class GalleryManager : MonoBehaviour {
    
    public static GalleryManager core;

    void Awake() {
        core = core == null ? this : core;
    }

    public GameObject galleryContent;
    public GameObject galleryPrefab;
    public GameObject activeImage;

    //Used to save gallery states
    public void saveGallery() {
        GallerySave gs = new GallerySave();
        gs.galleryImages = Database.core.gallery;

        SaveManager.core.JsonSave(gs, "gallery", ".set");
    }

    public void loadGallery() {
        GallerySave gs = new GallerySave();
        if (System.IO.File.Exists(Application.persistentDataPath + "/" + "gallery" + ".set")) {
            SaveManager.core.JsonLoad(gs, "gallery", ".set");
            Database.core.gallery = gs.galleryImages;
        }
    }

    //Used to load gallery states

    //Generating gallery
    public void updateGalleryWindow() {

        loadGallery();

        //Emptying gallery
        foreach (Transform tr in galleryContent.transform) {
            Destroy(tr.gameObject);
        }

        var gallery = Database.core.gallery;

        foreach (GalleryImage gi in gallery) {

            //Getting image
            var img = Database.core.getSpriteById(gi.galleryImageID);

            //Creating object instance
            var instance = Instantiate(galleryPrefab, galleryContent.transform);

            //Setting image
            instance.GetComponent<Image>().sprite = img;

            //Lock
            instance.transform.GetChild(0).gameObject.SetActive(gi.locked);
            instance.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Database.core.locked;

            //If the image is unlocked, we can click on the preview to view the image
            if (!gi.locked) {
                var b = instance.GetComponent<Button>();
                b.onClick.AddListener(delegate{ activeImage.GetComponent<Image>().sprite = img; activeImage.SetActive(true); });
            }
            

        }

    }

}

[System.Serializable]
public class GallerySave {
    public List<GalleryImage> galleryImages = new List<GalleryImage>();
}

// Code by Cination / Tsenkilidis Alexandros.