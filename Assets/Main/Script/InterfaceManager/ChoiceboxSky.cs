using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceboxSky : MonoBehaviour
{
    // This object: sky-imageset

    public Text[] choosedSkyTexts = new Text[6]; // some texts below these images
    public Image[] choosedSkyImages = new Image[6]; // skybox sample images
    public Button[] mainMenuButtons = new Button[4]; // button on main menu
    public Button mm; // start with manual mode
    public Button sd; // start with self-driving mode
    public Button backBtn; // button back
    public GameObject mainMenu; // the main menu
    private GameObject mainCamera; // the main camera
    public InputField massField; // inputfield for mass
    public InputField nameField; // inputfield for name
    public InterfaceManager globalInterfaceManager; // the master UI controller
  
    public Material[] skyboxs = new Material[6];
    private const int NORMAL_SKY = 0;
    private const int CLOUDY_SKY = 1;
    private const int FANTASY_SKY = 2;
    private const int DUSK_SKY = 3;
    private const int COSMIC_SKY = 4;
    private const int UNEARTH_SKY = 5;
    private int currentChoosedSky = 0;
    private float soundLevel; // a number between 0.0 - 1.0

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            // add event trigger for thoes UI elements
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            switch (i)
            {
                case NORMAL_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_NORMAL(); });
                    break;
                case CLOUDY_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_CLOUDY(); });
                    break;
                case FANTASY_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_FANTASY(); });
                    break;
                case DUSK_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_DUSK(); });
                    break;
                case COSMIC_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_COSMIC(); });
                    break;
                case UNEARTH_SKY:
                    entry.callback.AddListener((eventData) => { OnClick_UNEARTH(); });
                    break;
                default:
                    break;
            }
            choosedSkyTexts[i].GetComponent<EventTrigger>().triggers.Add(entry);
            choosedSkyImages[i].GetComponent<EventTrigger>().triggers.Add(entry);
        }
        mm.onClick.AddListener(OnClickManualModeButton);
        sd.onClick.AddListener(OnClickSelfDrivingModeButton);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        currentChoosedSky = NORMAL_SKY; // by default, it is normal sky.
        soundLevel = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentChoosedSky);
    }

    void OnClick_NORMAL()
    {
        // the text "Normal" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[NORMAL_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = NORMAL_SKY;
    }

    void OnClick_CLOUDY()
    {
        // the text "Cloudy" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[CLOUDY_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = CLOUDY_SKY;
    }

    void OnClick_FANTASY()
    {
        // the text "Fantasy" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[FANTASY_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = FANTASY_SKY;
    }

    void OnClick_DUSK()
    {
        // the text "Dusk" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[DUSK_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = DUSK_SKY;
    }

    void OnClick_COSMIC()
    {
        // the text "Cosmic" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[COSMIC_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = COSMIC_SKY;
    }

    void OnClick_UNEARTH()
    {
        // the text "Unearth" highlighted to grey
        choosedSkyTexts[currentChoosedSky].GetComponent<Text>().color = Color.white;
        choosedSkyTexts[UNEARTH_SKY].GetComponent<Text>().color = Color.grey;
        currentChoosedSky = UNEARTH_SKY;
    }

    void OnClickManualModeButton()
    {
        // it invoked when button "Manual mode" is pressed
        int skyNum = currentChoosedSky;
        float mass;
        try
        {
            mass = float.Parse(massField.text);
        }
        catch
        {
            massField.text = "Input should be a number";
            return;
        }
        string name = nameField.text;
        if (name.Equals("") || name.Equals("Can not be empty"))
        {
            nameField.text = "Can not be empty";
            return;
        }
        hideMainMenu();
        mainCamera.GetComponent<Skybox>().material = skyboxs[skyNum];
        globalInterfaceManager.setLock(false);
        globalInterfaceManager.setDroneName(name);
        globalInterfaceManager.setMass(mass);
        globalInterfaceManager.startGameWithMode(InterfaceManager.MANUAL_MODE);
        // initialize a simulation job

        /*
        Debug.Log("mass: " + mass);
        Debug.Log("name: " + name);
        Debug.Log("mode: manual mode");
        Debug.Log("sound: " + soundLevel + " %");
        */
    }

    void OnClickSelfDrivingModeButton()
    {
        // it invoked when button "Self-driving mode" is pressed
        int skyNum = currentChoosedSky;
        float mass;
        try
        {
            mass = float.Parse(massField.text);
        }
        catch
        {
            massField.text = "Input should be a number";
            return;
        }
        string name = nameField.text;
        if (name.Equals("") || name.Equals("Can not be empty"))
        {
            nameField.text = "Can not be empty";
            return;
        }
        hideMainMenu();
        mainCamera.GetComponent<Skybox>().material = skyboxs[skyNum];
        globalInterfaceManager.setLock(false);
        globalInterfaceManager.setDroneName(name);
        globalInterfaceManager.setMass(mass);
        globalInterfaceManager.startGameWithMode(InterfaceManager.SELF_DRIVING_MODE);

        /*
        Debug.Log("mass: " + mass);
        Debug.Log("name: " + name);
        Debug.Log("mode: self-driving mode");
        Debug.Log("sound: " + soundLevel + " %");
        */
    }
    
    void hideMainMenu()
    {
        // reinitialize the UI (menu)
        currentChoosedSky = NORMAL_SKY;
        mm.gameObject.SetActive(false);
        sd.gameObject.SetActive(false);
        massField.gameObject.SetActive(false);
        nameField.gameObject.SetActive(false);
        foreach(Button btn in mainMenuButtons)
        {
            btn.gameObject.SetActive(true);
        }
        this.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void setSoundLevel(float newSoundLevel)
    {
        this.soundLevel = newSoundLevel;
    }

    public float getSoundLevel()
    {
        return this.soundLevel;
    }
}
