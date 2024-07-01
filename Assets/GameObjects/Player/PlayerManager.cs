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
    string _currentState;
    string _lastState;
    [NonSerialized] public string _defaultState;
    static Dictionary<string, Action[]> _states = new Dictionary<string, Action[]>();

    // These actions are the changing code actually executed by the middlewares
    Action _mouseHover;
    Action _leftClick;
    Action _rightClick;

    PlayerInput _pInput;
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

    // Doesn't need context since it's not a key press
    private void MouseHoverMiddleware()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _lastHit, 100,  _clickableLayers))
        {
            _mouseHover();
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

    // Set the state to Default on Simulation Start
    private IEnumerator StartSimulation()
    {
        // We wait to be sure every stsate have been added
        byte offset = 3;
        while(offset-- == 0)
        {
            yield return null;
        }

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

    // TODO: Make it an actual getter, n'est ce pas Valentin
    public List<GameObject> GetDeck()
    {
        string name;
        for (int i = 0; i < 4; i++)
        {
            // It's getting worse
            //if (i % 4 == 1)
                name ="LaunchGrenadeModel";
            //else if (i % 4 == 2)
            //    name = "SimpleShot";
            //else if (i % 4 == 3)
            //    name = "PiercingShot";
            //else
            //    name = "Overdrive";


            //Card.Instantiate(name);

            
            //card.SetActive(false);
        }
        return CurrentRunInformations._deck;
    }

    private void PlayerDeath()
    {
        print("u dead");
    }
}
