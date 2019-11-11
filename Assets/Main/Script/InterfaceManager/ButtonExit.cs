using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExit : MonoBehaviour
{
    // Current object: Button Exit on main interface.

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Application.Quit(); // QUIT GAME, not work in editor mode
            EditorApplication.isPlaying = false; // close editor application
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
