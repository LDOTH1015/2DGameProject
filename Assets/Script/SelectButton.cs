using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectButton : MonoBehaviour
{
    public GameObject selectObject;
    public void SelectButtonClick()
    {
        selectObject.SetActive(true);
    }
}
