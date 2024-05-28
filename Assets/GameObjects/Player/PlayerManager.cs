using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public StatManager _health;
    byte _ultimateProgression;

    [NonSerialized] public Vector3 _virtualPos;
    CustomActions _input;

    [NonSerialized] public Coroutine _waitForConfirmationCoroutine;

    string _currentState;
    Action _currentAction;
    Dictionary<string, Action> _states;
    [NonSerialized] public string _defaultState;

    Func<UltiContext, bool> _ultimate;
    Action _middleware;

    PlayerManager() 
    {
        _states = new Dictionary<string, Action>();
        _defaultState = "movement";
    }
    private void Awake()
    {
        _input = new CustomActions();
        _input.Enable();
        _health = GetComponent<StatManager>();

        Class brawler = ClassFactory.Brawler();
        _ultimate = brawler._ultimate;

        _input.Main.LeftClick.performed += ctx => LeftClickMiddleware();

        StartCoroutine(StartSimulation());
    }

     ///!\ depracated function
     //This state change function disable the previous control listener state and enable the new one
    public void SetLeftClickTo(Action target)
    {
        _middleware = target;
    }

    private void LeftClickMiddleware()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, -1))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
        }
        _middleware();
    }

    public bool AddState(string name, Action func)
    {
        if(_states.ContainsKey(name) == false)
        {
            _states[name] = func;
            return true;
        }
        return false;
    }

    public bool SetToState(string name)
    {
        Action func;
        if(_states.TryGetValue(name, out func))
        {
            _currentState = name;
            _currentAction = func;
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

    public void UseUltimate()
    {
        _ultimate(new UltiContext());
    }

    public void ExecuteCurrentStateAction()
    {
        _currentAction();
    }

    public List<string> GetDeck()
    {
        List<string> deck = new List<string>();
        for(int i=0; i < 4; i++) 
        {
            deck.Add("LaunchGrenadeModel");
        }
        return deck;
    }
}
