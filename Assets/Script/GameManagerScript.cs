using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    public GameObject character1;
    private string names="";
    void Start()
    {
        PlayerPrefs.SetString("PlayerCharcter", "character1");
    }
    void Update()
    {
        names = "";
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject targetObject in targetObjects)
        {
            Text text = targetObject.GetComponent<Text>();

            if (text != null)
            {
                names += text.text + "\n";
            }
        }
    }
}
