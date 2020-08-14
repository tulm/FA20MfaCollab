using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Manages inventory resources */
public class ResourceManager : MonoBehaviour {

    public static ResourceManager core;

    [Tooltip("A list of resources in the player's inventory. Normally this list should be left empty.")]
    public List<Resource> inventoryResources = new List<Resource>();

    //Settings resource to a specific value
    public void setResource (string resourceID, float value) {
        Resource resource = inventoryResources.Find(x => x.resourceID == resourceID);
        
        if (resource == null) {
            resource = CopyResource(Database.core.resources.Find(x => x.resourceID == resourceID));
            resource.currentQuantity = value > resource.maxQuantity ? resource.maxQuantity : value;
            inventoryResources.Add(resource);
        } else {
            resource.currentQuantity = value > resource.maxQuantity ? resource.maxQuantity : value;
        }

    }

    //Add a certain quantity of an resource to the inventory
    public void addResource(string resourceID, float value) {
        Resource resource = inventoryResources.Find(x => x.resourceID == resourceID);
        
        if (resource == null) {
            resource = Database.core.resources.Find(x => x.resourceID == resourceID);
            resource.currentQuantity = value > resource.maxQuantity ? resource.maxQuantity : value;
            inventoryResources.Add(resource);
        } else {
            resource.currentQuantity = resource.currentQuantity + value > resource.maxQuantity ? resource.maxQuantity : resource.currentQuantity + value;
        }

    }

    //Removes a certain quantity of an resource from the inventory
    public void removeResource(string resourceID, float value, bool removeAll) {
        Resource resource = inventoryResources.Find(x => x.resourceID == resourceID);

        if (resource != null) {
            if (removeAll || resource.currentQuantity - value <= 0) {
                inventoryResources.Remove(resource);
            } else {
                resource.currentQuantity -= value;
            }
        }
    }

    //Checking for resource presence in the inventory
    public bool checkForResource(string resourceID, float requiredValue) {
        Resource resource = inventoryResources.Find(x => x.resourceID == resourceID);

        if (resource != null) {
            if (resource.currentQuantity >= requiredValue) {
                return true;
            }
        }

        return false;
    }

    Resource CopyResource(Resource resource) {
        var i = new Resource();

        i.currentQuantity = resource.currentQuantity;
        i.description = new List<String>();
        i.resourceIconID = resource.resourceIconID;
        i.resourceID = resource.resourceID;
        i.resourceName = resource.resourceName;
        i.maxQuantity = resource.maxQuantity;
        i.useFunction = resource.useFunction;
        
        foreach (String s in resource.description) {
            var n = new String();
            n.content = s.content;
            n.language = s.language;

            i.description.Add(n);
        }

        return i;
    }

    void Awake() {
        core = core == null ? this : null;
    }

}

// Code by Cination / Tsenkilidis Alexandros.