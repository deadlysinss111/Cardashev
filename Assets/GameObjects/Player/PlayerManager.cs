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
    Dictionary<string, Action[]> _states;
    [NonSerialized] public string _defaultState;

    Func<UltiContext, bool> _ultimate;
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
    }
    private void Awake()
    {
        _input = new CustomActions();
        _input.Enable();
        _health = GetComponent<StatManager>();

        Class brawler = ClassFactory.Brawler();
        _ultimate = brawler._ultimate;
        _ultimateProgression = 0;

        _rightClick = () => { };
        _input.Main.LeftClick.performed += ctx => LeftClickMiddleware();
        _input.Main.RightClick.performed += ctx => _rightClick();

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
        //Debug.Log("Called somewhere with name = " + name);

        Action[] func;
        if(_states.TryGetValue(name, out func))
        {
            Action[] exit;
            _states.TryGetValue(_currentState, out exit);
            exit[1]();
            _currentState = name;
            func[0]();
            return true;
        }
        return false;
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
            _ultimate(new UltiContext());
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
            deck.Add("LaunchGrenadeModel");
        }
        return deck;
    }
}
