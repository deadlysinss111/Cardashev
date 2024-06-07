using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditorInternal.VersionControl.ListControl;

public class PlayerManager : MonoBehaviour
{
    // In game visible fields

    [NonSerialized] public StatManager _health;
    byte _ultimateProgression;
    public int _goldAmount;

    static public List<Card> _deck;


    // Functional fields

    [NonSerialized] public Vector3 _virtualPos;
    CustomActions _input;

    string _currentState;
    string _lastState;
    Action _currentAction;
    Dictionary<string, Action[]> _states;
    [NonSerialized] public string _defaultState;

    Action _mouseHover;
    Action _leftClick;
    Action _rightClick;

    [NonSerialized] public RaycastHit _lastHit;
    [SerializeField] LayerMask _clickableLayers;

    PlayerManager() 
    {
        _states = new Dictionary<string, Action[]>();
        _defaultState = "movement";
        _currentState = "movement";
        _lastState = "movement";
    }
    private void Awake()
    {
        _input = new CustomActions();
        _input.Enable();
        _health = GetComponent<StatManager>();
        _deck = new List<Card>();

        _ultimateProgression = 0;
        _goldAmount = 100;

        _rightClick = () => { };
        _input.Main.LeftClick.performed += LeftClickMiddleware;
        _input.Main.RightClick.performed += RightClickMiddleware;
    }

    private void OnDestroy()
    {
        _input.Main.LeftClick.performed -= LeftClickMiddleware;
        _input.Main.RightClick.performed -= RightClickMiddleware;
    }

    private void Start()
    {
        StartCoroutine(StartSimulation());
    }

    //This state change function disable the previous control listener state and enable the new one
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

    private void RightClickMiddleware(InputAction.CallbackContext context)
    {
        _rightClick();
    }

    private void LeftClickMiddleware(InputAction.CallbackContext context)
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
    
    private void MouseHoverMiddleware()
    {
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _lastHit, 100, _clickableLayers))
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _lastHit))
        {
            _mouseHover();
        }
    }

    public bool AddState(string name, Action enter, Action exit)
    {
        if(_states.ContainsKey(name) == false)
        {
            Action[] buffer = new Action[2];
            buffer[0] = enter;
            buffer[1] = exit;
            _states[name] = buffer;
            return true;
        }
        return false;
    }

    public bool SetToState(string name)
    {
        Action[] func;
        if(_states.TryGetValue(name, out func))
        {
            _lastState = _currentState;
            Action[] exit;
            _states.TryGetValue(_currentState, out exit);
            exit[1]();
            _currentState = name;
            func[0]();
            return true;
        }
        return false;
    }

    public void SetToLastState()
    {
        SetToState(_lastState);
    }

    public void SetToDefault()
    {
        SetToState(_defaultState);
    }

    // We wait for every states to be added to the state machine and we set the default state
    IEnumerator StartSimulation()
    {
        int offset = 3;
        while(offset-- == 0)
        {
            yield return null;
        }
        SetToDefault();
    }

    public void UseUltimate()
    {
        if(_ultimateProgression >= 100)
        {
            Idealist._ultimate();
            _ultimateProgression = 0;
        }
    }

    // This method is useful when an object of the scene detects that the mouse went over them
    // When that happens, the concerned object calls this to avoid Raycasting every frame from MouseHoverMiddleware()
    public void TriggerMouseHovering()
    {
        MouseHoverMiddleware();
    }

    public List<string> GetDeck()
    {
        List<string> deck = new List<string>();
        for(int i=0; i < 4; i++) 
        {
            if (i%2 == 0)
                deck.Add("LaunchGrenadeModel");
            else
                deck.Add("TirSimple");
        }
        return deck;
    }
}
