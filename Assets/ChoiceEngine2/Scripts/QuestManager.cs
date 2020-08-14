using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Generating quests window */
public class QuestManager : MonoBehaviour {

    public GameObject questConatiner;
    public GameObject questContent;
    public GameObject questInspector;
    public Text questTitle;
    public Text questDescription;
    public Text questStatus;
    public GameObject questPrefab;

    //Generating quests window
    public void updateQuestsWindow() {

        //Destroying existing quests objects
        foreach (Transform t in questContent.transform) {
            Destroy(t.gameObject);
        }

        var quests = Database.core.quests;

        //Generating quests
        foreach (Quest q in quests) {
            var quest = Instantiate(questPrefab, questContent.transform);
            var b = quest.GetComponent<Button>();
            var title = quest.transform.GetChild(0).gameObject;
            var description = quest.transform.GetChild(1).gameObject;

            var tTitle = Database.core.getContentByLanguage(q.questName);
            var tDesc = Database.core.getContentByLanguage(q.questDescription);

            title.GetComponent<Text>().text = tTitle;
            description.GetComponent<Text>().text = tDesc;

            b.onClick.AddListener(delegate{
                questTitle.text = tTitle;
                questDescription.text = tDesc;
                if (q.assigned && q.questState != Quest.questStateEnum.Completed) {
                    questStatus.text = NodeManager.core.checkEnterConditions(q.completionConditions, true) ? "Status: Completed" : "Status: In-progress";
                } else if (q.questState == Quest.questStateEnum.Failed) {
                    questStatus.text = "Status: Failed";
                }

                questInspector.SetActive(true);

            });
        }

    }
}

// Code by Cination / Tsenkilidis Alexandros.