using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Main Menu functions */
public class MenuFunctions : MonoBehaviour {
    
    public static MenuFunctions core;
        
    [Tooltip("The index of the scene (in the Build settings) to be loaded when a new game is created.")]
    public int startingSceneIndex;

    [Tooltip("The index of the main menu scene (in the Build settings).")]
    public int mainMenuIndex;

    public GameObject mainMenuLoader;

    void Awake() {
        core = core == null ? this : core;
    }

    void Start() {
        if (LocalReferences.core.mainMenu.activeSelf) {
            SoundManager.core.PlayMainMenu();
        }
        LocalReferences.core.continueButton.interactable = PlayerPrefs.HasKey("Latest");
    }

    //Starting a new game
    public void newGame() {

        var notifications = LocalReferences.core.notificationsContainer;
        var txt = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
        var optionA = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();
        var optionB = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();

        //Setting text
        txt.text = "Start new game ?";

        //Button A listener
        optionA.onClick.RemoveAllListeners();
        optionA.onClick.AddListener(delegate { 
            notifications.SetActive(false);
        });

        //Button B listener
        optionB.onClick.RemoveAllListeners();
        optionB.onClick.AddListener(delegate {

            notifications.SetActive(false);
            LevelLoader.core.LoadLevel(startingSceneIndex, false, null);

        });

        notifications.SetActive(true);
        
    }

    //Loading latest save
    public void loadLatest() {

        if (PlayerPrefs.HasKey("Latest")) {

            var notifications = LocalReferences.core.notificationsContainer;
            var txt = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
            var optionA = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();
            var optionB = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();

            //Setting text
            txt.text = "Load game ?";

            //Button A listener
            optionA.onClick.RemoveAllListeners();
            optionA.onClick.AddListener(delegate { 
                notifications.SetActive(false);
            });

            //Button B listener
            optionB.onClick.RemoveAllListeners();
            optionB.onClick.AddListener(delegate {

                var latest = PlayerPrefs.GetString("Latest");
                SaveManager.core.GlobalLoad(latest);
                notifications.SetActive(false);


            });

            notifications.SetActive(true);

        }

    }

    //Closing main menu
    public void closeMainMenu() {
        
        if (LocalReferences.core.mainMenu.activeSelf) {
            LocalReferences.core.mainMenu.SetActive(false);
        }
        
    }

    //Opening main menu
    public void openMainMenu() {

        LocalReferences.core.settingsContainer.SetActive(false);
        LocalReferences.core.quickMenu.SetActive(false);
        
        LocalReferences.core.continueButton.interactable = PlayerPrefs.HasKey("Latest");
        
        var notifications = LocalReferences.core.notificationsContainer;
        var txt = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
        var optionA = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();
        var optionB = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();

        //Setting text
        txt.text = "Return to main menu?";

        //Button A listener
        optionA.onClick.RemoveAllListeners();
        optionA.onClick.AddListener(delegate { 
            notifications.SetActive(false);
        });

        //Button B listener
        optionB.onClick.RemoveAllListeners();
        optionB.onClick.AddListener(delegate { 

            var loader = Instantiate(mainMenuLoader);
            loader.GetComponent<MainMenuLoader>().mainMenuSceneIndex = mainMenuIndex;
            loader.GetComponent<MainMenuLoader>().LoadMainMenu();

        });

        notifications.SetActive(true);
    }

    //Exiting game
    public void quitGame() {

        var notifications = LocalReferences.core.notificationsContainer;
        var txt = notifications.transform.GetChild(1).gameObject.GetComponent<Text>();
        var optionA = notifications.transform.GetChild(3).gameObject.GetComponent<Button>();
        var optionB = notifications.transform.GetChild(2).gameObject.GetComponent<Button>();

        //Setting text
        txt.text = "Exit Game?";

        //Button A listener
        optionA.onClick.RemoveAllListeners();
        optionA.onClick.AddListener(delegate { 
            notifications.SetActive(false);
        });

        //Button B listener
        optionB.onClick.RemoveAllListeners();
        optionB.onClick.AddListener(delegate { 
            notifications.SetActive(false);
            Application.Quit();
        });

        notifications.SetActive(true);
    }


}

// Code by Cination / Tsenkilidis Alexandros.