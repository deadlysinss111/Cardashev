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
    private byte _ultimateProgression;

    [NonSerialized] public Vector3 _virtualPos;
    private PlayerInput _pInput;

    [NonSerialized] public Coroutine _waitForConfirmationCoroutine;

    private string _currentState;
    private Action _currentAction;
    private Dictionary<string, Action> _states;
    [NonSerialized] public string _defaultState;

    private Func<UltiContext, bool> _ultimate;
    private Action _mouseHover;
    private Action _leftClick;
    private Action _rightClick;

    public PlayerManager()
    {
        _states = new Dictionary<string, Action>();
        _defaultState = "movement";
    }

    private void Awake()
    {
        _pInput = GetComponent<PlayerInput>();
        _health = GetComponent<StatManager>();

        Class brawler = ClassFactory.Brawler();
        _ultimate = brawler._ultimate;
        _ultimateProgression = 0;

        _rightClick = () => { };
        _pInput.actions["LeftClick"].performed += ctx => LeftClickMiddleware();
        _pInput.actions["RightClick"].performed += ctx => _rightClick();

        StartCoroutine(StartSimulation());
    }

    private void OnEnable()
    {
        // Enable the input actions when the object is enabled
        _pInput.actions["Ultimate"].performed += OnUltimatePerformed;
        _pInput.actions["LeftClick"].performed += OnLeftClickPerformed;
        _pInput.actions["RightClick"].performed += OnRightClickPerformed;
    }

    private void OnDisable()
    {
        // Disable the input actions when the object is disabled
        _pInput.actions["Ultimate"].performed -= OnUltimatePerformed;
        _pInput.actions["LeftClick"].performed -= OnLeftClickPerformed;
        _pInput.actions["RightClick"].performed -= OnRightClickPerformed;
    }

    private void OnUltimatePerformed(InputAction.CallbackContext context)
    {
        UseUltimate();
    }

    private void OnMouseHoverPerformed(InputAction.CallbackContext context)
    {
        _mouseHover();
    }

    private void OnLeftClickPerformed(InputAction.CallbackContext context)
    {
        LeftClickMiddleware();
    }

    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        _rightClick();
    }

    public void SetLeftClickTo(Action target)
    {
        _leftClick = target;
    }

    public void SetHoverTo(Action target)
    {
        _mouseHover = target;
    }

    public void SetRightClickTo(Action target)
    {
        _rightClick = target;
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
        _leftClick();
    }

    public bool AddState(string name, Action func)
    {
        if (_states.ContainsKey(name) == false)
        {
            _states[name] = func;
            return true;
        }
        return false;
    }

    public bool SetToState(string name)
    {
        if (_states.TryGetValue(name, out var func))
        {
            _currentState = name;
            func();
            return true;
        }
        return false;
    }

    public void SetToDefault()
    {
        SetToState(_defaultState);
    }

    // We wait for every state to be added to the state machine and we set the default state
    private IEnumerator StartSimulation()
    {
        int offset = 2;
        while (offset-- > 0)
        {
            yield return null;
        }
        SetToDefault();
    }

    public void UseUltimate()
    {
        if (_ultimateProgression >= 100)
        {
            _ultimate(new UltiContext());
            _ultimateProgression = 0;
        }
    }

    public void ExecuteCurrentStateAction()
    {
        _mouseHover();
    }

    public List<string> GetDeck()
    {
        List<string> deck = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            deck.Add("LaunchGrenadeModel");
        }
        return deck;
    }
}
