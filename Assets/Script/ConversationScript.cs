using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationScript : MonoBehaviour
{
    public GameObject selectObject;
    public GameObject selectObject2;
    public void OnClick()
    {
        selectObject.SetActive(true);
        selectObject2.SetActive(false);
    }
}
