using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownAimRotation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer armRenderer;
    [SerializeField] private Transform armPivot1;

    [SerializeField] private SpriteRenderer characterRenderer1;

    private TopDownController _controller;

    private void Awake()
    {
        _controller = GetComponent<TopDownController>();
    }

    void Start()
    {
        _controller.OnLookEvent += OnAim;
    }

    public void OnAim(Vector2 newAimDirection)
    {
        // OnLook
        RotateArm(newAimDirection);
    }

    private void RotateArm(Vector2 direction)
    {
        string character = PlayerPrefs.GetString("PlayerCharcter", "PlayerCharcter");
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (character != null)
        {
            switch (character)
            {
                case "character1":
                    characterRenderer1.flipX = Mathf.Abs(rotZ) > 90f;
                    armPivot1.rotation = Quaternion.Euler(0, 0, rotZ);
                    break;
            }
        }
    }
}
