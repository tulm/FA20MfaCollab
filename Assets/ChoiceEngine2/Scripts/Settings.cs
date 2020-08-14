using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

/* This script contains the in-game settings. All settings can be also set during runtime. */
public class Settings : MonoBehaviour {

    public static Settings core;

    void Awake() {
        core = core == null ? this : core;
    }
    
    public Dropdown resolutionDropdown;
    public Dropdown modeDropdown;
    public Dropdown languageDropdown;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider dialogueVolume;
    public Slider SFxVolume;
    public Toggle skipUnseen;
    public Slider skipDelay;
    public Slider autoDelay;
    public Toggle animateText;
    public Slider animationDelay;

    public GameSettings settings = new GameSettings();

    private Resolution[] resolutions;

    //Used to apply chosen settings in the settings window and save the settings to the device.
    public void applySettings() {

        settings.autoDelay = autoDelay.value;
        settings.skipDelay = skipDelay.value;
        settings.skipUnseen = skipUnseen.isOn;
        settings.masterVolume = masterVolume.value;
        settings.musicVolume = musicVolume.value;
        settings.dialogueVolume = dialogueVolume.value;
        settings.resolution = resolutionDropdown.value;
        settings.SFxVolume = SFxVolume.value;
        settings.mode = modeDropdown.value;
        settings.language = languageDropdown.value;
        settings.animateText = animateText.isOn;
        settings.textAnimationDelay = animationDelay.value;

        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, false);

        //Updating volumes
        SoundManager.core.updateVolume();

        SaveManager.core.JsonSave(settings, "settings", ".set");

    }

    //Used to load settings from the device
    public void loadSettings(bool save) {

        SaveManager.core.JsonLoad(settings,"settings", ".set");

        autoDelay.value = settings.autoDelay;
        skipDelay.value = settings.skipDelay;
        skipUnseen.isOn = settings.skipUnseen;
        masterVolume.value = settings.masterVolume;
        musicVolume.value = settings.musicVolume;
        SFxVolume.value = settings.SFxVolume;
        dialogueVolume.value = settings.dialogueVolume;
        resolutionDropdown.value = settings.resolution;
        modeDropdown.value = settings.mode;
        languageDropdown.value = settings.language;
        animateText.isOn = settings.animateText;
        animationDelay.value = settings.textAnimationDelay;

        languageDropdown.RefreshShownValue();
        modeDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();

        if (save) {
            applySettings();
        }
        
    }

    //Initiating settings with base values
    public void initiateSettings() {

        //Getting unique resolutions
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        //Dropdown 1
        resolutionDropdown.options.Clear();

        for (int i = 0; i < resolutions.Length; i++) {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolutions[i].width + " x " + resolutions[i].height));

            //Setting current resolution
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                resolutionDropdown.value = i;
            }
        }

        //Dropdown 2
        string[] modes = {"Fullscreen", "Windowed"};

        modeDropdown.onValueChanged.RemoveAllListeners();
        modeDropdown.onValueChanged.AddListener(delegate {  Screen.fullScreen = modeDropdown.value == 0 ? true : false; });
        modeDropdown.options.Clear();

        for (int i = 0; i < modes.Length; i++) {
            modeDropdown.options.Add(new Dropdown.OptionData(modes[i]));
        }

        //Dropdown 3
        var languages = Database.core.languages;
        languageDropdown.onValueChanged.RemoveAllListeners();
        languageDropdown.onValueChanged.AddListener(delegate { settings.language = languageDropdown.value; });
        languageDropdown.options.Clear();

        foreach (string language in languages) {
            languageDropdown.options.Add(new Dropdown.OptionData(language));
        }

    }

    //Loading settings when game starts
    void Start() {
        initiateSettings();
        loadSettings(false);
    }

    [Tooltip("Save prefab")]
    public GameObject savePrefab;

    public void newSave() {

        var inputContainer = LocalReferences.core.inputContainer;
        var label = inputContainer.transform.GetChild(1).gameObject.GetComponent<Text>();
        var input = inputContainer.transform.GetChild(2).gameObject.GetComponent<InputField>();
        var confrimation = inputContainer.transform.GetChild(3).gameObject.GetComponent<Button>();

        label.text = "Name your save:";

        //Loading
        confrimation.onClick.RemoveAllListeners();
        confrimation.onClick.AddListener(delegate { 
            inputContainer.SetActive(false);
            SaveManager.core.GlobalSave(false, "", input.text);
            genSaveWindow();
        });

        inputContainer.SetActive(true);

    }

    //Generate saves window
    public void genSaveWindow() {
        
        var savesContainer = LocalReferences.core.savesContainer;

        //Destroying existing objects
        foreach(Transform t in savesContainer.transform) {
            Destroy(t.gameObject);
        }

        //Generating objects based on local files
        foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath)){

            //Filtering files
            if (Path.GetExtension(file) == ".save") {

                var instance = Instantiate(savePrefab, savesContainer.transform);
                var thumbnail = instance.transform.GetChild(0).gameObject.GetComponent<Image>();
                var button1 = instance.transform.GetChild(0).gameObject.GetComponent<Button>();
                var button2 = instance.transform.GetChild(2).gameObject.GetComponent<Button>();
                var buttonLabel = instance.transform.GetChild(1).gameObject.GetComponent<Text>();
                buttonLabel.text = Path.GetFileNameWithoutExtension(file) + " - " + File.GetCreationTime(file);

                var texPath = Application.persistentDataPath + "/" + Path.GetFileNameWithoutExtension(file) + ".png";

                if (File.Exists(texPath)) {
                    var tex = loadThumbnail(texPath);
                    thumbnail.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }

                //On click, overwrite save.
                button1.onClick.RemoveAllListeners();
                button1.onClick.AddListener(delegate { 

                    var notifications = LocalReferences.core.notificationsContainer;
                    var label = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
                    var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();
                    var optionB = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();

                    label.text = "Overwite save " + Path.GetFileName(file) + " ?";

                    //Saving
                    optionA.onClick.RemoveAllListeners();
                    optionA.onClick.AddListener(delegate {

                        notifications.SetActive(false);
                        SaveManager.core.GlobalSave (true, Path.GetFileNameWithoutExtension(file), "");
                        genSaveWindow();

                    });

                    optionB.onClick.RemoveAllListeners();
                    optionB.onClick.AddListener(delegate { 
                        notifications.SetActive(false);
                    });

                    notifications.SetActive(true);

                });

                //On click, delete save.
                button2.onClick.RemoveAllListeners();
                button2.onClick.AddListener(delegate {

                    var notifications = LocalReferences.core.notificationsContainer;
                    var label = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
                    var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();
                    var optionB = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();

                    label.text = "Delete " + Path.GetFileName(file) + " ?";

                    //Loading
                    optionA.onClick.RemoveAllListeners();
                    optionA.onClick.AddListener(delegate {
                        notifications.SetActive(false);
                        File.Delete(file);
                        genSaveWindow();
                    });

                    optionB.onClick.RemoveAllListeners();
                    optionB.onClick.AddListener(delegate { 
                        notifications.SetActive(false);
                    });

                    notifications.SetActive(true);

                });
            }
        }

    }

    public Texture2D loadThumbnail (string filePath) {
     
         Texture2D tex = null;
         byte[] fileData;
     
         if (File.Exists(filePath))     {
             fileData = File.ReadAllBytes(filePath);
             tex = new Texture2D(2, 2);
             tex.LoadImage(fileData);
         }

         return tex;
     }

    public void genLoadWindow() {

        var loadContainer = LocalReferences.core.loadContainer;

        foreach(Transform t in loadContainer.transform) {
            Destroy(t.gameObject);
        }

        foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath)){

            if (Path.GetExtension(file) == ".save") {

                var instance = Instantiate(savePrefab, loadContainer.transform);
                var thumbnail = instance.transform.GetChild(0).gameObject.GetComponent<Image>();
                var button1 = instance.transform.GetChild(0).gameObject.GetComponent<Button>();
                var button2 = instance.transform.GetChild(2).gameObject.GetComponent<Button>();
                var buttonLabel = instance.transform.GetChild(1).gameObject.GetComponent<Text>();
                buttonLabel.text = Path.GetFileNameWithoutExtension(file) + " - " + File.GetCreationTime(file);

                var texPath = Application.persistentDataPath + "/" + Path.GetFileNameWithoutExtension(file) + ".png";

                if (File.Exists(texPath)) {
                    var tex = loadThumbnail(texPath);
                    thumbnail.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }

                //On click, overwrite save.
                button1.onClick.RemoveAllListeners();
                button1.onClick.AddListener(delegate {

                    var notifications = LocalReferences.core.notificationsContainer;
                    var label = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
                    var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();
                    var optionB = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();

                    label.text = "Load " + Path.GetFileName(file) + " ?";

                    //Loading
                    optionA.onClick.RemoveAllListeners();
                    optionA.onClick.AddListener(delegate { 

                        notifications.SetActive(false);
                        SaveManager.core.GlobalLoad(Path.GetFileNameWithoutExtension(file));
                        LocalReferences.core.settingsContainer.SetActive(false);
                        genLoadWindow();

                    });

                    optionB.onClick.RemoveAllListeners();
                    optionB.onClick.AddListener(delegate { 
                        notifications.SetActive(false);
                    });

                    notifications.SetActive(true);

                });

                //On click, delete save.
                button2.onClick.RemoveAllListeners();
                button2.onClick.AddListener(delegate {

                    var notifications = LocalReferences.core.notificationsContainer;
                    var label = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
                    var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();
                    var optionB = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();

                    label.text = "Delete " + Path.GetFileName(file) + " ?";

                    //Loading
                    optionA.onClick.RemoveAllListeners();
                    optionA.onClick.AddListener(delegate {
                        notifications.SetActive(false);
                        File.Delete(file);
                        genLoadWindow();
                    });

                    optionB.onClick.RemoveAllListeners();
                    optionB.onClick.AddListener(delegate { 
                        notifications.SetActive(false);
                    });

                    notifications.SetActive(true);

                });

            }

        }

    }

    public void updateSaveButton() {
        LocalReferences.core.saveWindowButton.interactable = !LocalReferences.core.mainMenu.activeSelf;
    }

    //Opening settings on saves window
    public void openSavesWindow() {
        if (LocalReferences.core.mainMenu.activeSelf) {
            openLoadWindow();
            updateSaveButton();
        } else {
            genSaveWindow();
            updateSaveButton();
            LocalReferences.core.savesWindow.SetActive(true);
            LocalReferences.core.loadWindow.SetActive(false);
            LocalReferences.core.generalWindow.SetActive(false);
        }

    }

    //Opening settings on load window
    public void openLoadWindow() {

        genLoadWindow();
        updateSaveButton();
        LocalReferences.core.savesWindow.SetActive(false);
        LocalReferences.core.loadWindow.SetActive(true);
        LocalReferences.core.generalWindow.SetActive(false);
    }

    ////Opening settings on General Settings Window
    public void openGeneralSettings() {
        updateSaveButton();
        LocalReferences.core.savesWindow.SetActive(false);
        LocalReferences.core.loadWindow.SetActive(false);
        LocalReferences.core.generalWindow.SetActive(true);
    }

    //End Game
    public void endGame() {
        var notifications = LocalReferences.core.notificationsContainer;
        var label = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
        var optionA = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();
        var optionB = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();

        label.text = "Exit Game (Unsaved progress will be lost) ?";

        //Loading
        optionA.onClick.RemoveAllListeners();
        optionA.onClick.AddListener(delegate { 
            notifications.SetActive(false);
            Application.Quit();
        });

        optionB.onClick.RemoveAllListeners();
        optionB.onClick.AddListener(delegate { 
            notifications.SetActive(false);
        });

        notifications.SetActive(true);
    }
    
}

[System.Serializable]
public class GameSettings {

    [Range(0,1)]
    public float musicVolume;

    [Range(0,1)]
    public float dialogueVolume;

    [Range(0,1)]
    public float masterVolume;

    [Range(0,1)]
    public float SFxVolume;

    public bool skipUnseen;

    [Range(0.2f, 5)]
    public float autoDelay;

    [Range(0.1f, 1)]
    public float skipDelay;

    [HideInInspector]
    public int mode;

    [HideInInspector]
    public int resolution;

    [HideInInspector]
    public int language;

    public bool animateText;
    
    public float textAnimationDelay;

}

// Code by Cination / Tsenkilidis Alexandros.