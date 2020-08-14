using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
/* A node manager is responsible for hosting all the nodes to be displayed on the scene. */
public class NodeManager : MonoBehaviour {

    public static NodeManager core;

    [Tooltip("Stop all audio when loading a new level?")]
    public bool stopAudioWhenLoadingNewLevel;

    [HideInInspector]
    public bool processing;

    //When loading a new level, this variable can be used to load a custom node on that level.
    [HideInInspector]
    public string customStartingNode = "#-1";

    [HideInInspector]
    public GlobalSave previousLevelNode;

    [HideInInspector]
    public string previousNode = "#-1";

    Coroutine textAnimationRoutine;

    //Checking is the player is viable to enter node
    public bool checkEnterConditions(EntryConidtions entryConidtions, bool isQuest) {
        
        var resourceCheckPassed = true;
        var affectionCheckPassed = true;
        var statCheckPassed = true;
        var itemCheckPassed = true;

        var cont = true;

        foreach (resourceCheck rc in entryConidtions.resourceChecks) {
            resourceCheckPassed = ResourceManager.core.checkForResource(rc.resourceID, rc.requiredQuantity);
            if (!resourceCheckPassed) {
                cont = false;
                break;
            }
        }

        if (cont) {
            foreach (affectionCheck rc in entryConidtions.affectionChecks) {
                affectionCheckPassed = AffectionManager.core.checkAffection(Database.core.getCharacterById(rc.characterID), rc.requiredValue);
                if (!affectionCheckPassed) {
                    cont = false;
                    break;
                }
            }
        }

        if (cont) {
            foreach (itemCheck rc in entryConidtions.itemChecks) {
                itemCheckPassed = InventoryManager.core.checkForItem(rc.itemID, rc.requiredQuantity);
                if (!itemCheckPassed) {
                    cont = false;
                    break;
                }
            }   
        }
        
        if (cont) {
            foreach (statCheck rc in entryConidtions.statChecks) {
                statCheckPassed = StatsManager.core.checkStat(Database.core.getCharacterById(rc.characterID), rc.statID, rc.requiredValue);
                if (!statCheckPassed) {
                    cont = false;
                    break;
                }
            }    
        }

        if (cont) {
            foreach (questStateCheck rc in entryConidtions.questStateChecks) {
                var q = Database.core.getQuestById(rc.questID);
                if (q.questState != rc.questState) {
                    cont = false;
                    break;
                }
            }
        }

        if (cont) {
            if (!isQuest) {
                foreach (questCheck rc in entryConidtions.questChecks) {
                    statCheckPassed = checkEnterConditions(Database.core.getQuestById(rc.questID).completionConditions, true);
                    if (!statCheckPassed) {
                        cont = false;
                        break;
                    }
                }    
            }
        }


        return cont;

    }

    //Displaying node based on the provided node id.
    public void displayNode (string id) {

        //Getting next node
        var node = LevelManager.core.getNodeById(id);

        //Checking if the player is elligible to enter the node.
        var entryConidtions = node.entryConidtions;

        //If all enter conditions have been fulfilled
        if (checkEnterConditions(entryConidtions, false)) {

            //Actions on node enter
            var onNodeEnter = node.onNodeEnter;

            //Making sure that the save wasn't just loaded
            //If it was, it means that all events have already been handeled by the loading system
            if (!SaveManager.core.justLoaded) {
                
                //These are events that should note occure if the game has just loaded, since the save manager has already restored their effect.

                //Save node state?
                //If the node has noe previous state, and if there is a previous state to load, one will be loaded.
                if (node.previousNode == "#-1" && previousNode != "#-1") {
                    node.previousNode = previousNode; 
                    node.previousSave = previousLevelNode;
                }

                //Settings current node
                LevelManager.core.currentNode = id;

                //Adding node to visitedNodes
                if (Database.core.visitedNodes.Find(x => x.nodeID == id && x.levelIndex == SceneManager.GetActiveScene().buildIndex) == null) {
                    var j = new VisitedNode();
                    j.levelIndex = SceneManager.GetActiveScene().buildIndex;
                    j.nodeID = id;
                    j.content = Database.core.getContentByLanguage(node.text);
                    j.name = Database.core.getContentByLanguage(onNodeEnter.nameConfig.name);
                    Database.core.visitedNodes.Add(j);

                    if (Database.core.visitedNodesGlobal.Find(x => x.nodeID == id && x.levelIndex == SceneManager.GetActiveScene().buildIndex) == null) {
                        Database.core.visitedNodesGlobal.Add(j);
                        SkipAuto.core.saveVisited();
                    }
                    
                }

                //Applying modifications
                var mod = onNodeEnter.modificationsConfig;

                //Items
                foreach(modItem i in mod.modItems) {

                    switch (i.itemModType) {

                        case (modItem.modType.Add):
                            InventoryManager.core.addItem(i.itemID, i.value);
                            break;
                        case (modItem.modType.Set):
                            InventoryManager.core.setItem(i.itemID, i.value);
                            break;
                        case (modItem.modType.Subtract):
                            InventoryManager.core.removeItem(i.itemID, i.value, false);
                            break;
                        default:
                            InventoryManager.core.removeItem(i.itemID, i.value, true);
                            break;

                    }

                }

                //Affections
                foreach(modAffection i in mod.modAffections) {
                    var character = Database.core.getCharacterById(i.characterID);

                    switch (i.affectionModType) {
                        case (modAffection.modType.Add):
                            AffectionManager.core.increaseAffection(character, i.value);
                            break;
                        case (modAffection.modType.Set):
                            AffectionManager.core.setAffection(character, i.value);
                            break;
                        default:
                            AffectionManager.core.descreaseAffection(character, i.value);
                            break;
                    }

                }

                //Stats
                foreach(modStat i in mod.modStats) {
                    var character = Database.core.getCharacterById(i.characterID);

                    switch (i.statModType) {

                        case (modStat.modType.Add):
                            StatsManager.core.increaseStat(character, i.statID, i.value);
                            break;
                        case (modStat.modType.Set):
                            StatsManager.core.setStat(character, i.statID, i.value);
                            break;
                        default:
                            StatsManager.core.descreaseStat(character, i.statID, i.value);
                            break;
                    }

                }

                //Resources
                foreach(modResource i in mod.modResources) {

                    switch (i.resourceModType) {

                        case (modResource.modType.Add):
                            ResourceManager.core.addResource(i.resourceID, i.value);
                            break;
                        case (modResource.modType.Set):
                            ResourceManager.core.setResource(i.resourceID, i.value);
                            break;
                        case (modResource.modType.Subtract):
                            ResourceManager.core.removeResource(i.resourceID, i.value, false);
                            break;
                        default:
                            ResourceManager.core.removeResource(i.resourceID, i.value, true);
                            break;

                    }

                }

                //Quests Assignment
                foreach(modQuest i in mod.modQuests) {

                    Quest quest = Database.core.getQuestById(i.questID);

                    switch (i.questModType) {

                        case modQuest.modType.Assign:
                            quest.assigned = true;
                            break;
                        case modQuest.modType.Complete:
                            quest.questState = Quest.questStateEnum.Completed;
                            break;
                        case modQuest.modType.Fail:
                            quest.questState = Quest.questStateEnum.Failed;
                            break;

                    }

                }

                foreach (modCharacter i in mod.modCharacters) {
                    var character = Database.core.getCharacterById(i.characterID);
                    character.showInCharactersList = i.displayCharacter == modCharacter.displayEnum.Display ? true : false;
                }

                if (mod.modGallery.Count > 0) {
                    //Loading gallery to ensure that the previous state of the gallery is not overwritten
                    GalleryManager.core.loadGallery();
                }

                foreach (modGalleryImage i in mod.modGallery) {

                    //Updating gallery element states
                    var galleryImage = Database.core.getGalleryImageObjectById(i.galleryImageID);
                    galleryImage.locked = i.galleryImageStatus == modGalleryImage.galleryImageStatusEnum.Locked ? true : false;

                    //Saving updated gallery
                    GalleryManager.core.saveGallery();
                }

                //Adding audio to stop
                foreach (AudioInfoCompact toStop in onNodeEnter.audioConfig.audioToStop) {
                    SoundManager.core.StartCoroutine(SoundManager.core.StopAudio(toStop, false));
                }

                //Stopping all audio based on type
                if (onNodeEnter.audioConfig.stopAllAudio) {
                    SoundManager.core.StopAll(true, AudioInfo.audioTypeEnum.Music, false, onNodeEnter.audioConfig.audioToPlay);
                }

                if (onNodeEnter.audioConfig.stopAllMusic) {
                    SoundManager.core.StopAll(false, AudioInfo.audioTypeEnum.Music, false, onNodeEnter.audioConfig.audioToPlay);
                }

                if (onNodeEnter.audioConfig.stopAllDialogue) {
                    SoundManager.core.StopAll(false, AudioInfo.audioTypeEnum.Dialogue, false, onNodeEnter.audioConfig.audioToPlay);
                }

                if (onNodeEnter.audioConfig.stopAllSFx) {
                    SoundManager.core.StopAll(false, AudioInfo.audioTypeEnum.SFx, false, onNodeEnter.audioConfig.audioToPlay);
                }

                if (onNodeEnter.spawnConfig.despawnAll) {
                    SpawnManager.core.despawnAll();
                }

                //Adding audio to play
                foreach (AudioInfo toPlay in onNodeEnter.audioConfig.audioToPlay) {
                    toPlay.busy = false;
                    SoundManager.core.StartCoroutine(SoundManager.core.PlayAudio(toPlay));
                }

                //Spawning characters
                foreach (DisplayCharacter dc in onNodeEnter.spawnConfig.spawn) {
                    SpawnManager.core.spawn(dc);
                }

                //Despawning characters
                foreach (string dc in onNodeEnter.spawnConfig.despawn) {
                    SpawnManager.core.despawn(dc);
                }

                if (onNodeEnter.backgroundConfig.background == BackgroundConfig.backgroundEnum.New) {
                    BackgroundManager.core.ChangeBackground(onNodeEnter.backgroundConfig.backgroundID);
                }

            }

            //Triggering functions
            foreach(TriggerFunction f in onNodeEnter.functionsConfig.functionsToTrigger) {
                UserFunctions.core.Invoke(f.functionName, f.delay);
            }

            //Load a new level?
            if (onNodeEnter.loadLevelConfig.loadLevel) {
      
                //Hiding window before loading
                hideDialogueWindow(true, 0);

                //If we have chosen to stop all audio when loading a new level
                if (stopAudioWhenLoadingNewLevel) {
                   SoundManager.core.StopAll(true, AudioInfo.audioTypeEnum.Music, false, new List<AudioInfo>()); 
                }

                //Setting custom starting node for new level
                if (onNodeEnter.loadLevelConfig.levelStartingNode != "") {
                   customStartingNode = onNodeEnter.loadLevelConfig.levelStartingNode; 
                }

                LevelLoader.core.LoadLevel(onNodeEnter.loadLevelConfig.levelIndex, false, null);
                
            }

            //Hiding dialogue window
            if (onNodeEnter.dialogueWindowConfig.hideDialogueWindow == DialogueWindowConfig.hideEnum.Hide) {
                hideDialogueWindow(onNodeEnter.dialogueWindowConfig.hideIndefinitely, onNodeEnter.dialogueWindowConfig.hideDuration);
            }

            //Displaying a message
            if (onNodeEnter.notificationsConfig.displayMessage) {

                var notifications = LocalReferences.core.simpleNotificationsContainer;
                var txt = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
                var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();

                txt.text = Database.core.getContentByLanguage(onNodeEnter.notificationsConfig.message);

                optionA.onClick.RemoveAllListeners();
                optionA.onClick.AddListener(delegate { 
                    notifications.SetActive(false);
                });

                notifications.SetActive(true);

            }

            if (onNodeEnter.notificationsConfig.displayTitle) {
                
                if (titleRoutine != null) {
                    StopCoroutine(titleRoutine);
                }
                titleRoutine = StartCoroutine(displayTitle(Database.core.getContentByLanguage(onNodeEnter.notificationsConfig.title), onNodeEnter.notificationsConfig.titleDisplayTime));

            }
            
            //Request character/player name
            var inputContainer = LocalReferences.core.inputContainer;
            var label = inputContainer.transform.GetChild(1).gameObject.GetComponent<Text>();
            var inputField = inputContainer.transform.GetChild(2).gameObject.GetComponent<InputField>();
            var button = inputContainer.transform.GetChild(3).gameObject.GetComponent<Button>();

            if (onNodeEnter.nameConfig.requestName == NameConfig.requestNameEnum.Player) {
                label.text = "Enter your name: ";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { Database.core.playerName = inputField.text; inputContainer.SetActive(false); });
                inputContainer.SetActive(true);
            } else if (onNodeEnter.nameConfig.requestName == NameConfig.requestNameEnum.Character) {
                var c = Database.core.getCharacterById(onNodeEnter.nameConfig.characterID);
                label.text = "Enter " + Database.core.getContentByLanguage(c.name) + "'s new name: ";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { 
                    foreach (String language in c.name) {
                        language.content = inputField.text; 
                    }
                    inputContainer.SetActive(false); });
                inputContainer.SetActive(true);
            }

            //Displaying text area
            if (node.nodeType == Node.nodeTypeEnum.Text || node.nodeType == Node.nodeTypeEnum.Both) {

                //Showing dialogue window if it is not meant to be hidden
                if (node.onNodeEnter.dialogueWindowConfig.hideDialogueWindow == DialogueWindowConfig.hideEnum.Show) {
                    showDialogueWindow();   
                }

                var displayAvatar = onNodeEnter.avatarConfig.displayAvatar == AvatarConfig.avatarEnum.Show;
                var textArea = displayAvatar ? LocalReferences.core.textAreaA : LocalReferences.core.textAreaB;  
                var nameObject = displayAvatar ? LocalReferences.core.nameObjectA : LocalReferences.core.nameObjectB;
                var characterAvatar = LocalReferences.core.characterAvatar;
                var clickArea = displayAvatar ? LocalReferences.core.clickAreaA : LocalReferences.core.clickAreaB;

                //Disabling and enabling elements based on whether the avatar should be active
                LocalReferences.core.textAreaScrollA.SetActive(displayAvatar);
                LocalReferences.core.textAreaScrollB.SetActive(!displayAvatar);
                LocalReferences.core.nameObjectA.SetActive(displayAvatar);
                LocalReferences.core.nameObjectB.SetActive(!displayAvatar);
                LocalReferences.core.characterAvatar.SetActive(displayAvatar);
                LocalReferences.core.clickAreaA.SetActive(displayAvatar);
                LocalReferences.core.clickAreaB.SetActive(!displayAvatar);

                //Show avatar
                //Note that the actual avatar object is conditionally enabled in the section above.
                if (displayAvatar) {
                    characterAvatar.GetComponent<Image>().sprite = Database.core.getSpriteById(onNodeEnter.avatarConfig.AvatarID);
                }
                
                //Replacing all the tags in the string. Tags can be created and managed via the TagManager.cs script.
                var nodeText = TagManager.core.replaceTags(Database.core.getContentByLanguage(node.text));

                //Displaying text
                if (!Settings.core.settings.animateText || SkipAuto.core.skip) {
                    //If text should be shown normally
                    textArea.GetComponent<Text>().text = nodeText;
                } else {
                    //If text should be animated
                    textArea.GetComponent<Text>().text = "";

                    if (textAnimationRoutine != null) {
                        StopCoroutine(textAnimationRoutine);
                    }

                    //Calling animation enumerator
                    textAnimationRoutine = StartCoroutine(animateText(nodeText, textArea));
                }

                //Showing character/playing name
                if (onNodeEnter.nameConfig.displayName == NameConfig.nameEnum.Show) {
                    nameObject.GetComponent<Text>().text = Database.core.getContentByLanguage(onNodeEnter.nameConfig.name);
                    nameObject.SetActive(true);
                } else {
                    nameObject.SetActive(false);
                }

                
                //On click show next node if the node is supposed to be interactive
                var b = clickArea.GetComponent<Button>();
                if (node.textAreaInteractive && node.nodeType != Node.nodeTypeEnum.Both) {
                    b.onClick.RemoveAllListeners();

                    //Making sure that the entire text is being displayed
                    b.onClick.AddListener(delegate{
                        if (textArea.GetComponent<Text>().text == nodeText) {
                            displayNode(node.nextNodeRandom ? node.randomNodes[UnityEngine.Random.Range(0, node.randomNodes.Count)]: node.nextNodeId);
                        } else {
                            textArea.GetComponent<Text>().text = nodeText;
                        }
                        });
                } else {
                    b.onClick.RemoveAllListeners();
                }
                
            } else {
                LocalReferences.core.textAreaContainer.SetActive(false);
            }

            //Choices
            //Displaying choices based on node type
            var choiceAreaContainer = LocalReferences.core.choiceAreaContainer;
            if (node.nodeType == Node.nodeTypeEnum.Choice || node.nodeType == Node.nodeTypeEnum.Both) {

                var disp = node.onNodeEnter.avatarConfig.displayAvatar == AvatarConfig.avatarEnum.Show;
                var clickArea = disp ? LocalReferences.core.clickAreaA : LocalReferences.core.clickAreaB;

                //Making text are unclickable
                var b = clickArea.GetComponent<Button>();
                b.onClick.RemoveAllListeners();
                
                var choiceArea = LocalReferences.core.choiceArea;

                //Removing any existing options
                foreach (Transform t in choiceArea.transform) {
                    Destroy(t.gameObject);
                }

                //Displaying options
                foreach (Choice c in node.choices) {
                    var choice = Instantiate(LocalReferences.core.choicePrefab, choiceArea.transform);
                    choice.transform.GetChild(0).gameObject.GetComponent<Text>().text = Database.core.getContentByLanguage(c.choiceText);

                    var cb = choice.GetComponent<Button>();
                    cb.onClick.AddListener(delegate{ displayNode(c.choiceNodeID); });
                }

                choiceAreaContainer.SetActive(true);
            } else {
                choiceAreaContainer.SetActive(false);
            }

            //If the node if elligible to be a checkpoint, it will be saved as one.
            if (node.checkPoint) {

                //Recording node state. This will be used to load the previous node
                previousLevelNode = SaveManager.core.LocalSave();
                previousNode = LevelManager.core.currentNode;

            }

            //States are used to be able to go back to any previous node, independent of the level.
            //If the game has just been loaded, we do not want to save the states again.
            if (!SaveManager.core.justLoaded) {
                SaveManager.core.StateSave();
            }

            //Used by the level loader to let the node manager know that the game has just been loaded
            //This is done to prevent the node display function from performing actions that have already been perfromed before the game was saved.
            SaveManager.core.justLoaded = false;

        } else {

            SaveManager.core.justLoaded = false;
            displayNode(node.entryConidtions.failNodeID);
        }

        //Used by Auto/Skip to ensure that the node has been fully loaded before displaying the next node.
        processing = false;

    }

    //Hiding the dialogue window
    //If the "indefinitely" option is true, the window will not be shown until the next node is shown
    void hideDialogueWindow(bool indefinitely, float delay) {

        LocalReferences.core.textAreaContainer.SetActive(false);
        LocalReferences.core.toolContainer.SetActive(false);

        if (!indefinitely) {
            StartCoroutine(delayedShowDialogueWindow(delay));
        }

    }

    //Waiting before displaying the dialogue window.
    IEnumerator delayedShowDialogueWindow(float delay) {
        yield return new WaitForSeconds(delay);
        showDialogueWindow();
    }

    //Used to make the dialogue window visable.
    public void showDialogueWindow() {
        LocalReferences.core.textAreaContainer.SetActive(true);
        LocalReferences.core.toolContainer.SetActive(true);
    }

    //This is an enumerator used to animate text.
    IEnumerator animateText(string text, GameObject textArea) {
        var counter = 1;

        //While the the text is not entirely shown, animate text.
        while (counter <= text.Length && textArea.GetComponent<Text>().text != text) {
            textArea.GetComponent<Text>().text = text.Substring(0, counter);
            counter++;

            yield return new WaitForSeconds(Settings.core.settings.textAnimationDelay);
        }

    }

    Coroutine titleRoutine;
    IEnumerator displayTitle(string title, float t) {

        var titleScreenObject = LocalReferences.core.titleScreenObject;
        titleScreenObject.transform.GetChild(0).GetComponent<Text>().text = title;

        var c = titleScreenObject.GetComponent<Image>().color;
        c.a = 0;
        titleScreenObject.GetComponent<Image>().color = c;

        titleScreenObject.SetActive(true);

        while (c.a < 0.3) {
            c.a += 0.01f;
            titleScreenObject.GetComponent<Image>().color = c;
            c = titleScreenObject.GetComponent<Image>().color;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(t);

        while (c.a > 0) {
            c.a -= 0.01f;
            titleScreenObject.GetComponent<Image>().color = c;
            c = titleScreenObject.GetComponent<Image>().color;
            yield return new WaitForEndOfFrame();
        }

        titleScreenObject.SetActive(false);
    }

    void Awake() {
        //Instantiating a reference to the object
        core = core == null ? this: core;
    }
    
}

[System.Serializable]
public class Node {

    [Tooltip("A unique node ID, used to link and access nodes. This ID must be unique across all scenes.")]
    public string id;

    public enum nodeTypeEnum { Text, Choice, Both };

    [Tooltip("Display text, choices or both.")]
    public nodeTypeEnum nodeType = nodeTypeEnum.Text;

    [Tooltip("Should the next node be chosen randomly from a list?")]
    public bool nextNodeRandom;

    [Tooltip("A random node will be chosen from this list if \"nextNodeRandom\" is on.")]
    public List<string> randomNodes = new List<string>();

    [Tooltip("The ID of the next node to be displayed.")]
    public string nextNodeId;
    
    [Tooltip("Can the text are be clicked?")]
    public bool textAreaInteractive;

    [Tooltip("Making the node a checkpoint will allow the player to return to it when clicking the back button.")]
    public bool checkPoint;

    [Tooltip("The content of the text field.")]
    public List<String> text = new List<String>();

    [Tooltip("The list of available choices.")]
    public List<Choice> choices = new List<Choice>();

    [Tooltip("Conditions to enter the node.")]
    public EntryConidtions entryConidtions = new EntryConidtions();

    [Tooltip("Actions to take on node enter.")]
    public NodeEnter onNodeEnter = new NodeEnter();

    [HideInInspector]
    public string previousNode = "#-1";

    [HideInInspector]
    public GlobalSave previousSave;

    [HideInInspector]
    public bool seen = false;

    [HideInInspector]
    public bool hidden = false;

}

[System.Serializable]
public class Choice {

    [Tooltip("This is the text display on the choice button.")]
    public List<String> choiceText = new List<String>();

    [Tooltip("This is the node that will be accessed once the choice has been made.")]
    public string choiceNodeID;

}

//Events to happen on node enter
[System.Serializable]
public class NodeEnter {

    public enum menuEnum {Background, Audio, Avatar, Name, DialogueWindow, LevelLoading, Spawn, Modifications, Functions, Notifications, All}
    public menuEnum menu =  menuEnum.Background;

    public BackgroundConfig backgroundConfig = new BackgroundConfig();

    public AudioConfig audioConfig = new AudioConfig();

    public AvatarConfig avatarConfig = new AvatarConfig();

    public NameConfig nameConfig = new NameConfig();

    public DialogueWindowConfig dialogueWindowConfig = new DialogueWindowConfig();

    public LevelLoadingConfig loadLevelConfig = new LevelLoadingConfig();

    public SpawnConfig spawnConfig = new SpawnConfig();

    public ModificationsConfig modificationsConfig = new ModificationsConfig();

    public FunctionsConfig functionsConfig = new FunctionsConfig();

    public NotificationsConfig notificationsConfig = new NotificationsConfig();

}

[System.Serializable]
public class AvatarConfig {

    public enum avatarEnum { Show, Hide };

    [Tooltip("Display a character avatar ?")]
    public avatarEnum displayAvatar = avatarEnum.Hide;
    
    [Tooltip("The id of the avatar image in the database.")]
    public string AvatarID;

}

[System.Serializable]
public class AudioConfig {

    [Tooltip("A list of audio to play on node enter.")]
    public List<AudioInfo> audioToPlay = new List<AudioInfo>();

    [Tooltip("A list of audio to stop playing.")]
    public List<AudioInfoCompact> audioToStop = new List<AudioInfoCompact>();

    [Tooltip("Stop all audio ?")]
    public bool stopAllAudio;

    [Tooltip("Stop all audio of type 'music'?")]
    public bool stopAllMusic;

    [Tooltip("Stop all audio of type 'SFx'?")]
    public bool stopAllSFx;

    [Tooltip("Stop all audio of type 'Dialogue'?")]
    public bool stopAllDialogue;

}

[System.Serializable]
public class BackgroundConfig {

    public enum backgroundEnum { Current, New };

    [Tooltip("Keep current background or display a new one?")]
    public backgroundEnum background = backgroundEnum.Current;

    [Tooltip("The id of the new background image in the database.")]
    public string backgroundID;

}

[System.Serializable]
public class NotificationsConfig {

    [Tooltip("Display an alert on screen?")]
    public bool displayMessage;

    [Tooltip("The message to display.")]
    public List<String> message = new List<String>();

    [Tooltip("Display a title on screen?")]
    public bool displayTitle;

    [Tooltip("The title to display.")]
    public List<String> title = new List<String>();

    [Tooltip("The amount of time the title will be displayed on screen.")]
    public float titleDisplayTime;

}

[System.Serializable]
public class FunctionsConfig {

    [Tooltip("A list of functions to trigger on node enter (from UserFunctions.cs).")]
    public List<TriggerFunction> functionsToTrigger = new List<TriggerFunction>();

}

[System.Serializable]
public class DialogueWindowConfig {
    
    public enum hideEnum { Show, Hide };

    [Tooltip("Hide dialogue window on node enter ?")]
    public hideEnum hideDialogueWindow = hideEnum.Show;

    [Tooltip("Hide window indefinitely ?")]
    public bool hideIndefinitely;

    [Tooltip("Hide duration.")]
    public float hideDuration;

}

[System.Serializable]
public class LevelLoadingConfig {

    [Tooltip("Load new level?")]
    public bool loadLevel;

    [Tooltip("The index of the level to load.")]
    public int levelIndex;

    [Tooltip("The ID of the first node to load once the new level has loaded.")]
    public string levelStartingNode;

}

[System.Serializable]
public class NameConfig {
    
    public enum nameEnum { Show, Hide };

    [Tooltip("Display a character name ?")]
    public nameEnum displayName = nameEnum.Hide;
    
    [Tooltip("The id of the avatar image in the database.")]
    public List<String> name = new List<String>();

    public enum requestNameEnum { None, Character, Player };

    [Tooltip("Request player or character name?.")]
    public requestNameEnum requestName;

    [Tooltip("The id of the chatacter, in the database, whose name will be changed.")]
    public string characterID;

}

[System.Serializable]
public class SpawnConfig {

    [Tooltip("A list of characters sprites/modesl to display on node enter.")]
    public List<DisplayCharacter> spawn = new List<DisplayCharacter>();

    [Tooltip("A list of spawn points (ids in Canvas -> Spawns -> SpawnManager.cs) to clear.")]
    public List<string> despawn = new List<string>();

    [Tooltip("Clears all spawn points.")]
    public bool despawnAll;
    
}

[System.Serializable]
public class ModificationsConfig {

    [Tooltip("Manipulating items.")]
    public List<modItem> modItems = new List<modItem>();

    [Tooltip("Manipulating resources.")]
    public List<modResource> modResources = new List<modResource>();

    [Tooltip("Manipulating affections.")]
    public List<modAffection> modAffections = new List<modAffection>();

    [Tooltip("Manipulating stats.")]
    public List<modStat> modStats = new List<modStat>();

    [Tooltip("Manipulating quests.")]
    public List<modQuest> modQuests = new List<modQuest>();

    [Tooltip("Manipulating characters.")]
    public List<modCharacter> modCharacters = new List<modCharacter>();

    [Tooltip("Manipulating the image gallery.")]
    public List<modGalleryImage>  modGallery = new List<modGalleryImage>();

}

[System.Serializable]
public class EntryConidtions {

    public enum menuEnum {Items, Affections, Resources, Stats, Quests, All}

    [HideInInspector]
    public menuEnum menu =  menuEnum.Items;

    //Checking for items
    public List<itemCheck> itemChecks = new List<itemCheck>();

    //Checking for affections values
    public List<affectionCheck> affectionChecks = new List<affectionCheck>();

    //Checking for resources
    public List<resourceCheck> resourceChecks = new List<resourceCheck>();

    //Checking for stats
    public List<statCheck> statChecks = new List<statCheck>();

    //Check for quest completion
    public List<questCheck> questChecks = new List<questCheck>();

    //Check for quest states
    public List<questStateCheck> questStateChecks = new List<questStateCheck>();

    [Tooltip("The ID of the node to enter if any of the checks fail.")]
    public string failNodeID;

}

[System.Serializable]
public class questCheck {
    [Tooltip("The ID of the quest to be inspected for completion.")]
    public string questID;
}

[System.Serializable]
public class questStateCheck {
    [Tooltip("The ID of the quest to be checked.")]
    public string questID;
    
    [Tooltip("The state of the quest")]
    public Quest.questStateEnum questState;
}

[System.Serializable]
public class itemCheck {
    public string itemID;
    public float requiredQuantity;
}

[System.Serializable]
public class affectionCheck {
    public string characterID;
    public float requiredValue;
}

[System.Serializable]
public class resourceCheck {
    public string resourceID;
    public float requiredQuantity;
}

[System.Serializable]
public class statCheck {
    public string characterID;
    public string statID;
    public float requiredValue;
}

[System.Serializable]
public class modCharacter {

    [Tooltip("The ID of the character in the database.")]
    public string characterID;

    public enum displayEnum {Display, Hide}

    [Tooltip("Display/Hide the character in the characters window.")]
    public displayEnum displayCharacter;

}

[System.Serializable]
public class modItem {
    public string itemID;

    public enum modType { Add, Subtract, RemoveAll, Set }
    public modType itemModType;

    [Tooltip("The amount of item to subtract/add or set.")]
    public float value;

}

[System.Serializable]
public class modQuest {
    public string questID;
    public enum modType { Assign, Complete, Fail }
    public modType questModType;

}

[System.Serializable]
public class modResource {
    public string resourceID;

    public enum modType { Add, Subtract, RemoveAll, Set }
    public modType resourceModType;

    [Tooltip("The amount of resource to subtract/add or set.")]
    public float value;

}

[System.Serializable]
public class modAffection {
    public string characterID;

    public enum modType { Add, Subtract, RemoveAll, Set }
    public modType affectionModType;

    [Tooltip("The amount of affection to subtract/add or set.")]
    public float value;

}

[System.Serializable]
public class modStat {
    public string characterID;
    public string statID;

    public enum modType { Add, Subtract, RemoveAll, Set }
    public modType statModType;

    [Tooltip("The value by which to subtract/add or set the stat.")]
    public float value;

}

[System.Serializable]
public class modGalleryImage {

    [Tooltip("The id of the gallery image in the Gallery section of the Database.")]
    public string galleryImageID;
    public enum galleryImageStatusEnum { Locked, Unlocked }

    [Tooltip("The status of the image.")]
    public galleryImageStatusEnum galleryImageStatus;
}

[System.Serializable]
public class DisplayCharacter {

    [Tooltip("The id of the spawn point (Canvas -> Spawns -> SpawnManager.cs) of the characrer.")]
    public string spawnPointID;

    public enum spawnTypeEnum { Sprite, Model, Animation }

    [Tooltip("Type of object to spawned. Based on the choice, some of the following options will be disregarded.")]
    public spawnTypeEnum spawnType;

    [Tooltip("[Sprite] The id of the character sprite to display in the Database.")]
    public string spriteID;

    [Tooltip("[Model] The id of the character model (prefab) to spawn in the Database.")]
    public string modelID;

    [Tooltip("[Animation] A sequence of sprites that will be animated")]
    public List<Frame> animationFrames = new List<Frame>();

    [Tooltip("[Animation] Should the animation loop?")]
    public bool loopAnimation;

}

[System.Serializable]
public class Frame {
    [Tooltip("The ID of the sprite to be displayed.")]
    public string spriteID;

    [Tooltip("The delay before displaying the next sprite in the sequence.")]
    public float delay = 0.5f;
}

//This class allows the usage of the [Display()] tag. Please inspect the "DisplayDrawer.cs" script int the editor folder for more info.
[System.Serializable]
public class DisplayAttribute : PropertyAttribute {

    //Is the conditional variable in a list?
    public bool isInList;

    //Name of the list the variable belongs to.
    public string listName;

    //What is the name of the variable?
    //If the variable belongs to a class, the name would be classInstance.properyName, if not , just propertyName
    public string propertyName;

    //What is the value that the variable to have for the target to be displayed?
    public int[] propertyValues;

    public string targetProperty;

    public DisplayAttribute (bool i1, string i2, string i3, int[] i4, string i5) {
        this.isInList = i1;
        this.listName = i2;
        this.propertyName = i3;
        this.propertyValues = i4;
        this.targetProperty = i5;
    }
}

[System.Serializable]
public class TriggerFunction {

    [Tooltip("A function from UserFunctions.cs.")]
    public string functionName;

    [Tooltip("The delay before triggering the function.")]
    public float delay;
}

// Code by Cination / Tsenkilidis Alexandros.