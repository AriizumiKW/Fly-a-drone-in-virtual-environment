using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPicture : MonoBehaviour
{
    // change wall picture
    public bool ifUseRandomTexture;
    public Material[] pictures;
    LinkedList<GameObject> walls;
    void Start()
    {
        ifUseRandomTexture = false;
        walls = new LinkedList<GameObject>();
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children) // get reference of wall-object
        {
            // initialize List
            GameObject childObj = child.gameObject;
            if(childObj.tag == "horizontal-wall" || childObj.tag == "vertical-wall" || childObj.tag == "edge")
            {
                walls.AddLast(childObj);
            }
        }
    }

    void Update()
    {
        if (ifUseRandomTexture)
        {
            setRandomTextureForWalls();
            ifUseRandomTexture = false;
        }
    }

    // Update is called once per frame
    public void setRandomTextureForWalls()
    {
        foreach(GameObject wall in walls)
        {
            int randomNum = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 21.99f));
            wall.GetComponent<MeshRenderer>().material = pictures[randomNum];
        }
    }
}
