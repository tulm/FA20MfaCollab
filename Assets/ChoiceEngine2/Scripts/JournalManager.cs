using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Used to display the content of the traversed nodes. */
public class JournalManager : MonoBehaviour {

    public GameObject journalContent;
    public GameObject journalPrefab;

    [Tooltip("The number of characters in the preview.")]
    public int previewSize = 50;

    //Updating nodes window
    public void updateJournalWindow() {

        var entries = Database.core.visitedNodes;

        //Destroying previous journal objects
        foreach(Transform t in journalContent.transform) {
            Destroy(t.gameObject);
        }

        //Going through each journal entry.
        foreach (VisitedNode vn in entries) {
            
            if (vn.content != "") {
                
                var s = TagManager.core.replaceTags(vn.content);
                s = vn.name != "" ? vn.name + ": " + s : s;

                var entry = Instantiate(journalPrefab, journalContent.transform);
                var localPreviewSize = previewSize < s.Length - 1? previewSize : s.Length - 1;

                var postFix = localPreviewSize >= s.Length - 1 ? "" : " ...";
                entry.transform.GetChild(0).gameObject.GetComponent<Text>().text = s.Substring(0, localPreviewSize) + postFix;

                var b = entry.GetComponent<Button>();
                var originalSize = entry.GetComponent<RectTransform>().sizeDelta.y;
                
                //Enabling or disabling button based on whether a preview is avaiable.
                b.interactable = localPreviewSize >= s.Length - 1 ? false : true;
                
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(delegate{ expand(entry, s, originalSize); });

            }
            
        }

    }

    //Expand preview
    public void expand(GameObject entry, string s, float y) {
        entry.transform.GetChild(0).gameObject.GetComponent<Text>().text = s;

        var b = entry.GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{ minimize(entry, s, y);

        });

    }

    //Minimize preview
    public void minimize(GameObject entry, string s, float y) {
        var localPreviewSize = previewSize < s.Length - 1? previewSize : s.Length - 1;
        var postFix = localPreviewSize > s.Length - 1 ? "" : " ...";
        entry.transform.GetChild(0).gameObject.GetComponent<Text>().text = s.Substring(0, localPreviewSize) + postFix;

        var b = entry.GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate{ expand(entry, s, y); });
    }
}

// Code by Cination / Tsenkilidis Alexandros.