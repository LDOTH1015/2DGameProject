using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCScript : MonoBehaviour
{
    public Transform playerTransform;
    public float activationRange = 5.0f;
    public GameObject objectToActivate;
    public PlayerInput playerInput;

    private Transform thisTransform;
    private bool isActived = false;
    private bool isPlayerInRange = false;
    public GameObject Interact;

    private InputAction interactAction;

    void Start()
    {
        thisTransform = transform;

        // InputAction 직접 초기화
        interactAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
        interactAction.Enable();
        interactAction.performed += OnInteract;
    }

    void OnDestroy()
    {
        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    void Update()
    {
        if (GameManagerScript.Instance.isStageStarted || isActived)
            return;

        float distance = Vector3.Distance(thisTransform.position, playerTransform.position);
        isPlayerInRange = distance <= activationRange;

        if (!isPlayerInRange)
        {
            objectToActivate.SetActive(false);
            Interact.SetActive(false);
        }
        else 
        { 
            Interact.SetActive(true);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isPlayerInRange || isActived || GameManagerScript.Instance.isStageStarted)
            return;

        // 대화 시작
        string[] nullLines = null;
        objectToActivate.SetActive(true);
        objectToActivate.GetComponent<ConversationUI>().StartDialogue(nullLines);
        playerInput.enabled = false;
        StartCoroutine(ActivateThenCancelCoroutine());
    }

    private IEnumerator ActivateThenCancelCoroutine()
    {
        isActived = true;
        yield return new WaitForSeconds(30f);
        isActived = false;
        playerInput.enabled = true;
        objectToActivate.SetActive(false);
    }
}
