using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    public float _smoothSpeed = 8f;
    public Vector3 _offset;

    private Action _currentMode;
    private PlayerInput _pInput;
    private Vector2 _moveInput;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void Awake()
    {
        _currentMode = LockedMode;
        _pInput = GameObject.Find("Player").GetComponent<PlayerInput>();
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
