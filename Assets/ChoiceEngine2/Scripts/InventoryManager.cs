using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Managing inventory items */
public class InventoryManager : MonoBehaviour {

    public static InventoryManager core;

    [Tooltip("A list of items in the player's inventory. Normally this list should be left empty.")]
    public List<Item> inventoryItems = new List<Item>();

    //Settings item to a specific value
    public void setItem (string itemID, float value) {
        Item item = inventoryItems.Find(x => x.itemID == itemID);
        
        if (item == null) {
            item = CopyItem(Database.core.items.Find(x => x.itemID == itemID));
            item.currentQuantity = value > item.maxQuantity ? item.maxQuantity : value;
            inventoryItems.Add(item);
        } else {
            item.currentQuantity = value > item.maxQuantity ? item.maxQuantity : value;
        }

    }

    //Add a certain quantity of an item to the inventory
    public void addItem(string itemID, float value) {
        Item item = inventoryItems.Find(x => x.itemID == itemID);
        
        if (item == null) {
            item = Database.core.items.Find(x => x.itemID == itemID);
            item.currentQuantity = value > item.maxQuantity ? item.maxQuantity : value;
            inventoryItems.Add(item);
        } else {
            item.currentQuantity = item.currentQuantity + value > item.maxQuantity ? item.maxQuantity : item.currentQuantity + value;
        }

    }

    //Removes a certain quantity of an item from the inventory
    public void removeItem(string itemID, float value, bool removeAll) {
        Item item = inventoryItems.Find(x => x.itemID == itemID);

        if (item != null) {
            if (removeAll || item.currentQuantity - value <= 0) {
                inventoryItems.Remove(item);
            } else {
                item.currentQuantity -= value;
            }
        }
    }

    //Checking for item presence in the inventory
    public bool checkForItem(string itemID, float requiredValue) {
        Item item = inventoryItems.Find(x => x.itemID == itemID);

        if (item != null) {
            if (item.currentQuantity >= requiredValue) {
                return true;
            }
        }

        return false;
    }

    //Custom deep copy
    Item CopyItem(Item item) {
        var i = new Item();

        i.currentQuantity = item.currentQuantity;
        i.description = new List<String>();
        i.itemIconID = item.itemIconID;
        i.itemID = item.itemID;
        i.itemName = item.itemName;
        i.maxQuantity = item.maxQuantity;
        i.useFunction = item.useFunction;
        
        foreach (String s in item.description) {
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