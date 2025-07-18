using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : TopDownController
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>().normalized;
        CallMoveEvent(moveInput);
    }

    public void OnLook(InputValue value)
    {
        Vector2 newAim = value.Get<Vector2>();
        Vector2 worldPos = _camera.ScreenToWorldPoint(newAim);
        newAim = (worldPos - (Vector2)transform.position).normalized;
        CallLookEvent(newAim);
    }
    public void OnFire(InputValue value)
    {
        CallFireEvent();
    }

    public void OnEvasion(InputValue value)
    {
        CallEvasionEvent();
    }

    public void OnInventory(InputValue value) 
    {
        CallInvenEvent();
    }

    public void OnPotion(InputValue value)
    {
        CallInvenEvent();
    }
}