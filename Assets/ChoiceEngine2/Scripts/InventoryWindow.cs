using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Script used to generate inventory window */
public class InventoryWindow : MonoBehaviour {
    
    public GameObject inventoryContent;
    public GameObject itemInspector;
    public Image itemIcon;
    public Text itemName;
    public Text itemDescription;
    public GameObject itemPrefab;
    public Button discardButton;
    public Button useButton;

    public void updateInventoryWindow() {

        foreach (Transform t in inventoryContent.transform) {
            Destroy(t.gameObject);
        }

        //Get all items currently in the inventory.
        var items = InventoryManager.core.inventoryItems;

        //For each item in the inventory.
        foreach (Item i in items) {
            var item = Instantiate(itemPrefab, inventoryContent.transform);
            item.transform.GetChild(0).gameObject.GetComponent<Text>().text = i.currentQuantity.ToString();
            item.GetComponent<Image>().sprite = Database.core.getSpriteById(i.itemIconID);
            item.GetComponent<Button>().onClick.AddListener(delegate { updateInspector(i); itemInspector.SetActive(true); });
        }

    }

    //Updating item inspector with item information.
    private void updateInspector(Item i) {

        itemIcon.sprite = Database.core.getSpriteById(i.itemIconID);
        itemName.text = Database.core.getContentByLanguage(i.itemName);
        itemDescription.text = Database.core.getContentByLanguage(i.description);

        discardButton.onClick.RemoveAllListeners();
        useButton.onClick.RemoveAllListeners();

        discardButton.onClick.AddListener(delegate{ 
            InventoryManager.core.removeItem(i.itemID, 0, true);
            itemInspector.SetActive(false);
            updateInventoryWindow();
        });

        useButton.onClick.AddListener(delegate { 
            UserFunctions.core.Invoke(i.useFunction, 0f);
            itemInspector.SetActive(false);
            updateInventoryWindow();
        });
        
    }

}

// Code by Cination / Tsenkilidis Alexandros.