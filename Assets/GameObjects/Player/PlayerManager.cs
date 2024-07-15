using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditorInternal.VersionControl.ListControl;

public class PlayerManager : MonoBehaviour
{
    /*
     FIELDS 
    */
    // Fields that will appear in the Room UI
    [NonSerialized] public StatManager _statManagerRef;
    byte _ultimateProgression;

    // State related
    public string _currentState;
    string _lastState;
    [NonSerialized] public string _defaultState;
    static Dictionary<string, Action[]> _states = new Dictionary<string, Action[]>();
    bool _disablingState;

    // These actions are the changing code actually executed by the middlewares
    Action _mouseHover = () => { /*print("hover wtf");*/ };
    Action _leftClick = () => { /*print("left wtf");*/ };
    Action _rightClick = () => { /*print("right wtf");*/ };

    [NonSerialized] public PlayerInput _pInput;
    [NonSerialized] public Vector3 _virtualPos;
    [NonSerialized] public RaycastHit _lastHit;
    [SerializeField] LayerMask _clickableLayers;    // TODO: Implement that


    /*
     EVENTS
    */
    public UnityEvent _UeOnDefeat;


    /*
     METHODS
    */
    // Pre-Awake constructor
    PlayerManager()
    {
        _defaultState = "movement";
        _currentState = "movement";
        _lastState = "movement";
    }

    private void Awake()
    {
        // Event Subscribing
        _UeOnDefeat.AddListener(PlayerDeath);

        // Fetching some refs
        _pInput = GetComponent<PlayerInput>();
        _statManagerRef = GetComponent<StatManager>();

        // Deck initialization

        _ultimateProgression = 0;
    }

    private void Start()
    {
        StartCoroutine(StartSimulation());
    }

    //This state change function disable the previous control listener state and enable the new one
    private void OnEnable()
    {
        // Subscribe the input actions when the object is enabled
        _pInput.actions["Ultimate"].performed += OnUltimatePerformed;
        _pInput.actions["LeftClick"].performed += OnLeftClickPerformed;
        _pInput.actions["RightClick"].performed += OnRightClickPerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe the input actions when the object is disabled
        _pInput.actions["Ultimate"].performed -= OnUltimatePerformed;
        _pInput.actions["LeftClick"].performed -= OnLeftClickPerformed;
        _pInput.actions["RightClick"].performed -= OnRightClickPerformed;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe the input actions when the object is destroyed
        _pInput.actions["Ultimate"].performed -= OnUltimatePerformed;
        _pInput.actions["LeftClick"].performed -= OnLeftClickPerformed;
        _pInput.actions["RightClick"].performed -= OnRightClickPerformed;
    }

    private void Update()
    {
        TriggerMouseHovering();
    }


    // ------
    // MIDDLEWARE FEST
    // ------

    // These 3 are meant to be bound to player's inputs
    private void OnUltimatePerformed(InputAction.CallbackContext context)
    {
        UseUltimate();
    }

    private void OnLeftClickPerformed(InputAction.CallbackContext context)
    {
        LeftClickMiddleware(context);
    }

    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        RightClickMiddleware(context);
    }

    // These 2 middlewares need a context to be compatible with input remaping (CustomInput package)
    private void RightClickMiddleware(InputAction.CallbackContext context)
    {
        _rightClick();
    }

    private void LeftClickMiddleware(InputAction.CallbackContext context)
    {
        bool hasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, -1);
        
        if (hasHit)
        {
            _disablingState = false;
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
        }
        _leftClick();
        _disablingState = false;
    }

    // Doesn't need context since it's not a key press
    private void MouseHoverMiddleware()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _lastHit, 100,  _clickableLayers))
        {
            if (_disablingState)
            {
                SetToLastState();
            }

            _disablingState = false;
            _mouseHover();

            SetCursorToCorrespondingOne();
        }
        else
        {
            if(false == _disablingState && _currentState == "movement")
            {
                SetToState("Empty");
                _disablingState = true;
                GI.SetCursorTo(GI.CursorRestriction.VOID);
            }
        }
    }

    private void SetCursorToCorrespondingOne()
    {
        if(_lastHit.transform.gameObject.TryGetComponent(out Enemy enemy ) )
        {
            if (enemy.IsSelectable)
                GI.SetCursorTo(GI.CursorRestriction.S_ENEMIES);
            else
                GI.SetCursorTo(GI.CursorRestriction.ENEMIES);
            return;
        }
        
        if(_lastHit.transform.gameObject.TryGetComponent(out Interactible interactible))
        {
            if (interactible._inRange)
                GI.SetCursorTo(GI.CursorRestriction.S_INTERACTIBLES);
            else
                GI.SetCursorTo(GI.CursorRestriction.INTERACTIBLES);
            return;
        }

        if (_lastHit.transform.gameObject.TryGetComponent(out Tile tile))
        {
            if (tile.IsSelectable)
                GI.SetCursorTo(GI.CursorRestriction.S_TILES);
            else
                GI.SetCursorTo(GI.CursorRestriction.TILES);
            return;
        }
    }


    // ------
    // ACTIONS SETTERS
    // ------

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


    // ------
    // METHODS TO ADD OR SWITCH THE STATE
    // ------
    static public bool AddState(string name, Action enter, Action exit)
    {
        if (_states.ContainsKey(name) == false)
        {
            Action[] buffer = new Action[2];
            buffer[0] = enter;
            buffer[1] = exit;
            _states[name] = buffer;
            return true;
        }
        return false;
    }
    
    static public void AddOrOverrideState(string name, Action enter, Action exit)
    {
        Action[] buffer = new Action[2];
        buffer[0] = enter;
        buffer[1] = exit;
        _states[name] = buffer;
    }

    public bool SetToState(string name)
    {
        //print("set to : " + name);
        if (name == _currentState) return false;

        if(_states.TryGetValue(name, out Action[] func))
        {
            if(_currentState!="Empty")
                _lastState = _currentState;
            Action[] exit;
            GI.ResetCursorValues();
            _states.TryGetValue(_currentState, out exit);
            exit[1]();
            _currentState = name;
            func[0]();
            return true;
        }
        print("missed state change");
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

    // Set the state to Default on Simulation Start
    private IEnumerator StartSimulation()
    {
        // We wait to be sure every stsate have been added
        byte offset = 3;
        while(offset-- == 0)
        {
            yield return null;
        }

        AddState("Empty",
            () => {
                SetLeftClickTo(() => { print("huh"); });
                SetRightClickTo(() => { });
                SetHoverTo(() => { });
            }, () => { });
        SetToDefault();
    }

    public void UseUltimate()
    {
        if (_ultimateProgression >= 100)
        {
            Idealist._instance.Ultimate();
            _ultimateProgression = 0;
        }
    }

    // This method is useful when an object of the scene detects that the mouse went over them
    // When that happens, the concerned object calls this to avoid Raycasting every frame from MouseHoverMiddleware()
    public void TriggerMouseHovering()
    {
        MouseHoverMiddleware();
    }
    
    public void TriggerMouseExit()
    {
        Action[] exit;
        _states.TryGetValue(_currentState, out exit);
        exit[1]();
        SetLeftClickTo(() => { });
        SetRightClickTo(() => { });
    }

    // TODO: Make it an actual getter, n'est ce pas Valentin
    public List<GameObject> GetDeck()
    {
        return CurrentRunInformations._deck;
    }

    private void PlayerDeath()
    {
        print("u dead");
    }
}
