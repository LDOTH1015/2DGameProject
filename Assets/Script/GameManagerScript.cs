using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    public GameObject character1;
    [SerializeField] private Image staminaFillamount;
    private string names="";
    private PlayerStatus status;
    void Start()
    {
        PlayerPrefs.SetString("PlayerCharcter", "character1");
        status = PlayerStatus.Instance;
    }
    void Update()
    {
        names = "";
        staminaFillamount.fillAmount = status.curruntStamina / status.maxStamina;
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
