using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    public float _smoothSpeed = 8f;
<<<<<<< HEAD
    Vector3 _offset;
    Quaternion _rotation;
    Action _currentMode;
    CustomActions _input;
=======
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
>>>>>>> A--Rebind

    // Clockwise positions around player starting behind him
    Vector3[] _offsetPositions;
    byte _offsetID;

    private void Awake()
    {
        _currentMode = LockedMode;
<<<<<<< HEAD
        _input = new CustomActions();
        _input.Enable();
        _input.Main.SpaceBar.performed += ctx => ChangeMode();
        _input.CameraControls.RotateToLeft.performed += ctx => RotateToLeft();
        _input.CameraControls.RotateToRight.performed += ctx => RotateToRight();
        _rotation = transform.rotation;
        _offsetPositions = new Vector3[4];
        _offsetPositions[0] = new Vector3(0, 15, -10);
        _offsetPositions[1] = new Vector3(10, 15, 0);
        _offsetPositions[2] = new Vector3(0, 15, 10);
        _offsetPositions[3] = new Vector3(-10, 15, 0);
        _offsetID = 0;
        _offset = _offsetPositions[0];
=======
        _pInput = GameObject.Find("Player").GetComponent<PlayerInput>();
>>>>>>> A--Rebind
    }

    private void OnEnable()
    {
        
        _pInput.actions["CameraMode"].performed += OnChangeModePerformed;
        _pInput.actions["CameraMove"].performed += OnMovePerformed;
        _pInput.actions["CameraMove"].canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        if (_pInput != null)
        {
            _pInput.actions["CameraMode"].performed -= OnChangeModePerformed;
            _pInput.actions["CameraMove"].performed -= OnMovePerformed;
            _pInput.actions["CameraMove"].canceled -= OnMoveCanceled;
        }
    }

    private void Update()
    {
        _currentMode();
    }

    private void LockedMode()
    {
        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, _rotation, _smoothSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
    }

    private void UnlockedMode()
    {
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
        transform.Translate(move * Time.deltaTime * _smoothSpeed, Space.World);
    }

    private void ChangeMode()
    {
        if (_currentMode == LockedMode)
        {
            _currentMode = UnlockedMode;
        }
        else
        {
            _currentMode = LockedMode;
        }
    }

<<<<<<< HEAD
    private void RotateToLeft()
    {
        if(_offsetID == 0)
        {
            _offsetID = 3;
        }
        else
        {
            --_offsetID;
        }
        _offset = _offsetPositions[_offsetID];

        // Rotation transformations
        _rotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _rotation *= Quaternion.AngleAxis(90, Vector3.up);
        _rotation *= Quaternion.AngleAxis(45, Vector3.right);
    }
    private void RotateToRight()
    {
        if(_offsetID == 3)
        {
            _offsetID = 0;
        }
        else
        {
            ++_offsetID;
        }
        _offset = _offsetPositions[_offsetID];

        // Rotation transformations
        _rotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _rotation *= Quaternion.AngleAxis(-90, Vector3.up);
        _rotation *= Quaternion.AngleAxis(45, Vector3.right);
    }


    /* Some really cool maths rotations with a shift bug left
       May be usefull if we want to let the player move around freely
    
    private void RotateToLeft()
    {
        // Movement transformations
        Vector3 gap = _target.position - transform.position;
        gap = Quaternion.AngleAxis(90, Vector3.up) * gap;
        _offset.x = gap.x;
        _offset.z = gap.z;

        // Rotation transformations
        _rotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _rotation *= Quaternion.AngleAxis(-90, Vector3.up);
        _rotation *= Quaternion.AngleAxis(45, Vector3.right);
    }

    private void RotateToRight()
    {
        // Movement transformations
        Vector3 gap = _target.position - transform.position;
        gap = Quaternion.AngleAxis(-90, Vector3.up) * gap;
        _offset.x = gap.x ;
        _offset.z = gap.z ;

        // Rotation transformations
        _rotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _rotation *= Quaternion.AngleAxis(90, Vector3.up);
        _rotation *= Quaternion.AngleAxis(45, Vector3.right);
    }
    */
=======
    private void OnChangeModePerformed(InputAction.CallbackContext context)
    {
        ChangeMode();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    public void StartRebinding(string actionName)
    {
        var action = _pInput.actions[actionName];
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found.");
            return;
        }

        
        if (_rebindingOperation != null)
        {
            _rebindingOperation.Dispose();
        }

        _rebindingOperation = action.PerformInteractiveRebinding()
            .OnComplete(operation => RebindComplete())
            .OnCancel(operation => RebindCancel())
            .Start();
    }

    private void RebindComplete()
    {
        Debug.Log("Rebind complete!");
        _rebindingOperation.Dispose();
        _rebindingOperation = null;
    }

    private void RebindCancel()
    {
        Debug.Log("Rebind canceled!");
        _rebindingOperation.Dispose();
        _rebindingOperation = null;
    }

    public string GetBindingDisplayString(string actionName)
    {
        var action = _pInput.actions[actionName];
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found.");
            return string.Empty;
        }

        return action.GetBindingDisplayString();
    }
>>>>>>> A--Rebind
}
