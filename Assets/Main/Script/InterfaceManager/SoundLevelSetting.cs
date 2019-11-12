using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundLevelSetting : MonoBehaviour
{
    // This gameobject: Slider-sound

    public ChoiceboxSky manager; // Sky-imageset
    public Text soundText; // Text-sound
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = this.gameObject.GetComponent<Slider>();
        slider.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        manager.setSoundLevel(slider.value);
        soundText.text = Math.Floor(slider.value * 100).ToString() +  " %";
    }
}
