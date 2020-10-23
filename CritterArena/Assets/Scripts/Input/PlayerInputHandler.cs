using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 axisInput;

    private Action<Vector3> OnMoveAction;
    private Action OnDashAction;


    public void RegisterOnMoveAction(Action<Vector3> onMoveAction)
    {
        OnMoveAction += onMoveAction;
    }

    public void RegisterOnDashAction(Action onDashAction)
    {
        OnDashAction += onDashAction;
    }

    public void OnMove(CallbackContext context)
    {
        axisInput = context.ReadValue<Vector2>();
        OnMoveAction?.Invoke(axisInput.Vector3XZ());
    }

    public void OnDash(CallbackContext context)
    {
        if (context.started)
        {
            OnDashAction?.Invoke();
        }
    }
}
