using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRecord : MonoBehaviour
{
    public Text filePath;
    // Start is called before the first frame update
    void Start()
    {
        filePath.gameObject.SetActive(false);
        this.GetComponent<Button>().onClick.AddListener(onClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onClick()
    {
        filePath.gameObject.SetActive(true);
        filePath.text = "please open file directory:\"" + Application.persistentDataPath + "\"to find any record.";
        StopCoroutine("showText2False");
        StartCoroutine("showText2False");
    }

    private IEnumerator showText2False()
    {
        yield return new WaitForSeconds(3.5f);
        filePath.gameObject.SetActive(false);
    }
}
