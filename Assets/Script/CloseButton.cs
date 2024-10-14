using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public GameObject selectObject;
    public void OnClick()
    {
        selectObject.SetActive(false);
    }
}
