using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public StatManager _health;
    byte _ultimateProgression;

    [NonSerialized] public Vector3 _virtualPos;
    CustomActions _input;
    Action<InputAction.CallbackContext> _lastLeftClickAction;

    [NonSerialized] public Coroutine _waitForConfirmationCoroutine;

    string _currentState;
    Dictionary<string, Action> _states;
    [NonSerialized] public string _defaultState;

    PlayerManager() 
    {
        _lastLeftClickAction = ctx => { };
        _states = new Dictionary<string, Action>();
        _defaultState = "movement";
    }
    private void Awake()
    {
        _input = new CustomActions();
        _input.Enable();
        _health = GetComponent<StatManager>();

        StartCoroutine(StartSimulation());
    }

    // /!\ depracated function
    // This state change function disable the previous control listener state and enable the new one
    public void SetLeftClickTo(Action target)
    {
        _input.Main.LeftClick.performed -= _lastLeftClickAction;
        _lastLeftClickAction = ctx => { target(); };
        _input.Main.LeftClick.performed += _lastLeftClickAction;
    }

    public void AddState(string name, Action func)
    {
        if(_states.ContainsKey(name) == false)
        {
            _states[name] = func;
        }
    }

    public bool SetToState(string name)
    {
        Action func;
        if(_states.TryGetValue(name, out func))
        {
            _currentState = name;
            SetLeftClickTo(_states[name]);
            return true;
        }
        return false;
    }

    public void SetToDefult()
    {
        SetToState(_defaultState);
    }

    // We wait for every states to be added to the state machine and we set the default state
    IEnumerator StartSimulation()
    {
        int offset = 2;
        while(offset-- == 0)
        {
            yield return null;
        }
        SetToDefult();
    }

}
