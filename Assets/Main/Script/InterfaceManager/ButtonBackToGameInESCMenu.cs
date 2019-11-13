using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBackToGameInESCMenu : MonoBehaviour
{
    // This gameobject: Button-escbacktogame

    public GameObject escMenu; // gameobject: ESCMenu
    public InterfaceManager globalInterfaceManager;
    public GameObject[] soundSettingUIElement = new GameObject[3]; // Text-soundLevel, Text-sound, Slider-sound


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
        globalInterfaceManager.setLock(false);
        globalInterfaceManager.updateSoundLevel();
        escMenu.SetActive(false);
        foreach (GameObject obj in soundSettingUIElement)
        {
            obj.SetActive(false);
        }
    }
}
