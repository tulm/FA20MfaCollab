using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*This script is responsible for updating the resource window */
public class ResourceWindow : MonoBehaviour {
    public GameObject resourceContent;
    public GameObject resourcePrefab;

    public void updateResourceWindow() {

        foreach (Transform t in resourceContent.transform) {
            Destroy(t.gameObject);
        }

        var resources = ResourceManager.core.inventoryResources;

        foreach (Resource r in resources) {
            var resource = Instantiate(resourcePrefab, resourceContent.transform);
            resource.transform.GetChild(0).gameObject.GetComponent<Text>().text = Database.core.getContentByLanguage(r.resourceName) + ": " + r.currentQuantity.ToString();
            resource.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Database.core.getSpriteById(r.resourceIconID);
        }

    }

}

// Code by Cination / Tsenkilidis Alexandros.