using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManagerScript : MonoBehaviour
{
    public Text nameText;
    public Text timeText;
    public GameObject character1;
    public GameObject character2;
    void Start()
    {
        nameText.text =  PlayerPrefs.GetString("PlayerName", "Player");
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
        string character = PlayerPrefs.GetString("PlayerCharcter", "PlayerCharcter");
        if (character != null)
        {
            switch (character)
            {
                case "character1":
                    character1.SetActive(true);
                    character2.SetActive(false);
                    break;
                case "character2":
                    character1.SetActive(false);
                    character2.SetActive(true);
                    break;
            }
        }
        
    }
    void Update()
    {
        nameText.text = PlayerPrefs.GetString("PlayerName", "Player");
        string character = PlayerPrefs.GetString("PlayerCharcter", "PlayerCharcter");
        if (character != null)
        {
            switch (character)
            {
                case "character1":
                    character2.transform.position = character1.transform.position;
                    break;
                case "character2":
                    character1.transform.position = character2.transform.position;
                    break;
            }
        }
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
    }
}
