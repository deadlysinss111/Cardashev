using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public StatManager _health;
    [NonSerialized] public Vector3 _virtualPos;
    CustomActions _input;
    Action<InputAction.CallbackContext> _lastLeftClickAction;

    [NonSerialized] public Coroutine _waitForConfirmationCoroutine;

    private void Awake()
    {
        _health = GetComponent<StatManager>();
        _input = new CustomActions();
        _input.Enable();
        _lastLeftClickAction = ctx => { };
        GameObject.Find("Player").GetComponent<PlayerController>().SetToMovementState();
    }

    // This state change function disable the previous control listener state and enable the new one
    public void SetLeftClickTo(Action target)
    {
        _input.Main.LeftClick.performed -= _lastLeftClickAction;
        _lastLeftClickAction = ctx => { target(); };
        _input.Main.LeftClick.performed += _lastLeftClickAction;
    }

    // we expect a cool state machine that enable / disable controls affectation :)
}
