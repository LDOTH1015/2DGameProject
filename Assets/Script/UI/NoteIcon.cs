using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteIcon : MonoBehaviour
{
    [SerializeField] private GameObject select3in1;
    // Start is called before the first frame update
    public void StageStart()
    {
        select3in1.SetActive(true);
    }
}
