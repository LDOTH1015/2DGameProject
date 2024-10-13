using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public GameObject character1;
    public GameObject Character2;
    public GameObject selectObject;

    void Start()
    {
        
    }

    public void OnButtonClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button1":
                character1.SetActive(true);
                Character2.SetActive(false);
                selectObject.SetActive(false);
                PlayerPrefs.SetString("PlayerCharcter", "character1");
                break;
            case "Button2":
                character1.SetActive(false);
                Character2.SetActive(true);
                selectObject.SetActive(false);
                PlayerPrefs.SetString("PlayerCharcter", "character2");
                break;
        }
    }
}
