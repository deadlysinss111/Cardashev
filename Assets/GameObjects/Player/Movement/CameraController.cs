using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    public float _smoothSpeed = 8f;

    Vector3 _curOffset;
    Quaternion _curRotation;
    Action _currentMode;

    // New input system's fields
    PlayerInput _pInput;
    Vector2 _moveInput;
    InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    // Clockwise positions around player starting behind him
    Vector3[] _offsetPositions;
    byte _offsetID;

    private void Awake()
    {
        // New input system's awakening
        _currentMode = LockedMode;
        _pInput = GameObject.Find("Player").GetComponent<PlayerInput>();

        // Pre-determined camera angles
        _curRotation = transform.rotation;
        _offsetPositions = new Vector3[4];
        _offsetPositions[0] = new Vector3(0, 15, -10);
        _offsetPositions[1] = new Vector3(10, 15, 0);
        _offsetPositions[2] = new Vector3(0, 15, 10);
        _offsetPositions[3] = new Vector3(-10, 15, 0);
        _offsetID = 0;
        _curOffset = _offsetPositions[0];
    }

    private void OnEnable()
    {
        _pInput.actions["CameraMode"].performed += OnChangeModePerformed;
        _pInput.actions["CameraMove"].performed += OnMovePerformed;
        _pInput.actions["CameraMove"].canceled += OnMoveCanceled;
        _pInput.actions["CameraRotateLeft"].performed += RotateToLeft;
        _pInput.actions["CameraRotateRight"].performed += RotateToRight;
    }

    private void OnDisable()
    {
        if (_pInput != null)
        {
            _pInput.actions["CameraMode"].performed -= OnChangeModePerformed;
            _pInput.actions["CameraMove"].performed -= OnMovePerformed;
            _pInput.actions["CameraMove"].canceled -= OnMoveCanceled;
            _pInput.actions["CameraRotateLeft"].performed -= RotateToLeft;
            _pInput.actions["CameraRotateRight"].performed -= RotateToRight;
        }
    }

    private void Update()
    {
        _currentMode();
    }

    private void LockedMode()
    {
        Vector3 desiredPosition = _target.position + _curOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.unscaledDeltaTime);
        transform.position = smoothedPosition;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, _curRotation, _smoothSpeed * Time.unscaledDeltaTime);
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

    private void RotateToLeft(InputAction.CallbackContext context)
    {
        if (_offsetID == 0)
        {
            _offsetID = 3;
        }
        else
        {
            --_offsetID;
        }
        _curOffset = _offsetPositions[_offsetID];
        //Debug.Log(_curOffset);

        // Rotation transformations
        _curRotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _curRotation *= Quaternion.AngleAxis(90, Vector3.up);
        _curRotation *= Quaternion.AngleAxis(45, Vector3.right);
    }

    private void RotateToRight(InputAction.CallbackContext context)
    {
        if (_offsetID == 3)
        {
            _offsetID = 0;
        }
        else
        {
            ++_offsetID;
        }
        _curOffset = _offsetPositions[_offsetID];

        //Debug.Log(_curOffset);

        // Rotation transformations
        _curRotation *= Quaternion.AngleAxis(-45, Vector3.right);
        _curRotation *= Quaternion.AngleAxis(-90, Vector3.up);
        _curRotation *= Quaternion.AngleAxis(45, Vector3.right);
    }

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
}
