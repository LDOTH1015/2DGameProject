using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNameButton : MonoBehaviour
{
    public GameObject selectObject;
    public Text nameText;
    // Start is called before the first frame update
    public void OnClickButton()
    {
        string playerName = nameText.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        selectObject.SetActive(false);
    }
}
