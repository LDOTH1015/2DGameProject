using System;
using System.Collections.Generic;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2> OnLookEvent;
    public event Action OnFireEvent;
    public event Action OnEvasionEvent;

    public void CallMoveEvent(Vector2 direction)
    {
        OnMoveEvent?.Invoke(direction);
    }

    public void CallLookEvent(Vector2 direction)
    {
        OnLookEvent?.Invoke(direction);
    }
    public void CallFireEvent()
    {
        OnFireEvent?.Invoke();
    }

    public void CallEvasionEvent()
    {
        OnEvasionEvent?.Invoke();
    }
}
