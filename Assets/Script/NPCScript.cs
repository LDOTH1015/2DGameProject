using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public Transform playerTransform;

    public float activationRange = 5.0f;

    public GameObject objectToActivate;

    private Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(thisTransform.position, playerTransform.position);

        if (distance <= activationRange)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            objectToActivate.SetActive(false);
        }
    }
}
