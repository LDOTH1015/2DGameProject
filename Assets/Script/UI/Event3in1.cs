using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Event3in1 : MonoBehaviour
{
    [SerializeField]private PlayerInput NPCScript;
    [SerializeField] private GameObject UI3in1;
    [SerializeField]private StageManager StageManager;
    public void OnClick()
    {
        NPCScript.enabled = true;
        StageManager.StartStage(); ;
        GameManagerScript.Instance.isStageStarted = true;
        UI3in1.SetActive(false);
    }
}
