using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBackToMenuInESCMenu : MonoBehaviour
{
    // This gameobject: Button-escbacktomenu

    public GameObject escMenu; // gameobject: ESCMenu
    public InterfaceManager globalInterfaceManager;
    public GameObject[] soundSettingUIElement = new GameObject[3]; // Text-soundLevel, Text-sound, Slider-sound
    public GameObject mainMenu; // gameobject: MainMenu


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(onClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onClick()
    {
        // users press "Back to menu" in esc interface
        globalInterfaceManager.stopGame();
        mainMenu.SetActive(true);
        escMenu.SetActive(false);
        foreach (GameObject obj in soundSettingUIElement)
        {
            obj.SetActive(false);
        }
    }
}
