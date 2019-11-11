using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetting : MonoBehaviour
{
    // Current object: Button Setting on main interface.

    public GameObject[] mainInterfaceButtons = new GameObject[4];
    // button-start, button-record, button-setting, button-exit
    public GameObject[] elementsInSettingMenu = new GameObject[4];
    // button-back, slider-sound, text-sound, text-soundlevel

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
        foreach (GameObject btn in mainInterfaceButtons)
        {
            btn.SetActive(false);
        }
        foreach (GameObject element in elementsInSettingMenu)
        {
            element.SetActive(true);
        }
    }
}
