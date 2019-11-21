using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    // This gameobject: InterfaceManager
    public const int NOT_START = 0;
    public const int MANUAL_MODE = 1;
    public const int SELF_DRIVING_MODE = 2;

    public GameObject escMenu; // gameobject: ESCMenu
    public GameObject[] soundSetting = new GameObject[3]; // Text-soundLevel, Text-sound, Slider-sound
    public Slider soundLevelSlider;
    public PowerfulEngine engine; // PowerfulEngine of GameObject "Drone"

    // UIText in debug interface
    public Text droneNameText;
    public Text modeText;
    public Text massText;
    public Text possitionText;
    public Text speedText;
    public Text accelerationText;
    public Text rotationText;
    public Text gameLockText;
    public Text soundLevelText;
    public Text chatBarText;

    private bool gameLock; // if the system stop running. As usual, it is true when press ESC.
    private int gameMode; // 0 means task not start, 1 is manual mode, 2 is self-driving mode
    private float soundLevel; // a number between 0.0f - 1.0f

    private string droneName; // name of the drone. It is given by users.
    private float mass; // the mass of drone.

    // Start is called before the first frame update
    void Start()
    {
        gameLock = true;
        gameMode = 0;

        initSoundSlider();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameLock)
        {
            //Debug.Log("Game Pause");
        }
        else
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                // show the esc interface
                setLock(true);
                escMenu.SetActive(true);
                foreach(GameObject obj in soundSetting)
                {
                    obj.SetActive(true);
                }
            }
        }
        //updateDebugInfoText();
    }

    void updateDebugInfoText()
    {
        
    }

    void initSoundSlider()
    {
        soundLevelSlider.gameObject.SetActive(true);
        soundLevelSlider.value = 0.3f;
        soundLevel = 0.3f;
        soundLevelSlider.gameObject.SetActive(false);
    }

    public void setLock(bool newState)
    {
        this.gameLock = newState;
        gameLockText.text = "GameLock: " + gameLock.ToString();
    }

    public bool getLock()
    {
        return this.gameLock;
    }

    public void setDroneName(string droneName)
    {
        this.droneName = droneName;
        droneNameText.text = "Name: " + droneName;
    }

    public string getDroneName()
    {
        return this.droneName;
    }

    public void startGameWithMode(int mode)
    {
        if(mode != MANUAL_MODE && mode != SELF_DRIVING_MODE)
        {
            return;
        }
        this.gameMode = mode;
        modeText.text = "Mode: " + mode.ToString();
        
        updateSoundLevel();
    }

    public int getGameMode()
    {
        return this.gameMode;
    }

    public void setMass(float mass)
    {
        this.mass = mass;
        engine.resetDrone(mass);

        massText.text = "Mass: " + mass.ToString();
    }

    public float getMass()
    {
        return this.mass;
    }

    public void stopGame()
    {
        Debug.Log("Game Stop");
        gameMode = 0;
        modeText.text = "Mode: " + gameMode.ToString();
    }

    public void updateSoundLevel()
    {
        soundLevel = soundLevelSlider.value;
        soundLevelText.text = soundLevel.ToString();
    }

    public void updateRotationText(float x, float y, float z)
    {
        rotationText.text = "Rotation: " + "(" + x.ToString() + " ," + y.ToString() + " ," + z.ToString() + ").";
    }

    public void updateVelocityAcceleration(Vector3 v, Vector3 a)
    {
        string vText = "Velocity: " + "(" + v.x.ToString("0.00") + ", " + v.y.ToString("0.00") + ", " + v.z.ToString("0.00") + ")";
        string aText = "Acceleration: " + "(" + a.x.ToString("0.00") + ", " + a.y.ToString("0.00") + ", " + a.z.ToString("0.00") + ")";
        speedText.text = vText;
        accelerationText.text = aText;
    }
}
