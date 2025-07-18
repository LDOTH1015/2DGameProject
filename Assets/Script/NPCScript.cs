using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCScript : MonoBehaviour
{
    public Transform playerTransform;

    public float activationRange = 5.0f;

    public GameObject objectToActivate;

    private Transform thisTransform;

    public PlayerInput playerInput;

    private bool isActived = false;

    void Start()
    {
        thisTransform = transform;
    }

    void Update()
    {
        if (!GameManagerScript.Instance.isStageStarted && !isActived)
        {
            float distance = Vector3.Distance(thisTransform.position, playerTransform.position);

            if (distance <= activationRange)
            {
                string[] nullLines = null;
                objectToActivate.SetActive(true);
                objectToActivate.GetComponent<ConversationUI>().StartDialogue(nullLines);
                playerInput.enabled = false;
                StartCoroutine(ActivateThenCancelCoroutine());
            }
            else
            {
                objectToActivate.SetActive(false);
                playerInput.enabled = true;
            }
        }
    }

    private IEnumerator ActivateThenCancelCoroutine()
    {
        isActived = true;

        yield return new WaitForSeconds(30f);

        isActived = false;
    }
}
