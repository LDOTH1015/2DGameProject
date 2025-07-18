using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Event3in1 : MonoBehaviour
{
    [SerializeField]private PlayerInput NPCScript;
    [SerializeField] private GameObject UI3in1;
    public void OnClick()
    {
        NPCScript.enabled = true;
        SpwanSystem.Instance.StartSpwaner();
        GameManagerScript.Instance.isStageStarted = true;
        UI3in1.SetActive(false);
    }
}
