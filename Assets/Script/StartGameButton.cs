using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StartGameButton : MonoBehaviour
{
    public Text nameText;
    // Start is called before the first frame update
    public void StartGame()
    {
        string playerName = nameText.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        SceneManager.LoadScene("MainScene");
    }
}
