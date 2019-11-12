using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStart : MonoBehaviour
{
    // Current object: Button Back on setting interface, which is the sub-menu of main interface.

    public GameObject[] mainInterfaceButtons = new GameObject[4];
    public GameObject[] elementsOnNewMenu = new GameObject[6];
    // button-start, button-record, button-setting, button-exit

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
        foreach (GameObject element in elementsOnNewMenu)
        {
            element.SetActive(true);
        }
    }
}