using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public Transform playerTransform;

    public float activationRange = 5.0f;

    public GameObject objectToActivate;
    public GameObject objectToActivateTrigger;

    private Transform thisTransform;
    // Start is called before the first frame update
    void Start()
    {
        thisTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(thisTransform.position, playerTransform.position);

        if (distance <= activationRange)
        {
            if (!objectToActivateTrigger.activeSelf)
            {
                objectToActivate.SetActive(true);
            } 
        }
        else
        {
            objectToActivate.SetActive(false);
        }
    }
}
