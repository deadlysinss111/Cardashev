using System;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    public float _smoothSpeed = 15f;

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
    float _zoomScale = 1.0f;

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
        _pInput.actions["CameraRotateLeft"].performed += RotateToRight;
        _pInput.actions["CameraRotateRight"].performed += RotateToLeft;
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
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                _zoomScale -= 0.1f;
            }
            else
            {
                _zoomScale += 0.1f;
            }
            _zoomScale = Math.Clamp(_zoomScale, 0.5f, 2.5f);
        }
    }

    private void LockedMode()
    {
        Vector3 desiredPosition = _target.position + _curOffset * _zoomScale; ;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.unscaledDeltaTime);
        transform.position = smoothedPosition;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, _curRotation, _smoothSpeed * Time.unscaledDeltaTime);
        transform.rotation = smoothedRotation;
    }

    private void UnlockedMode()
    {
        Vector3 move;
        
        switch (_offsetID)
        {
            case 0:
                move = new Vector3(_moveInput.y, 0, -_moveInput.x);
                break;
            case 1:
                move = new Vector3(_moveInput.x, 0, _moveInput.y);
                break;
            case 2:
                move = new Vector3(-_moveInput.y, 0, _moveInput.x);
                break;
            case 3:
                move = new Vector3(-_moveInput.x, 0, -_moveInput.y);
                break;
            default:
                move = new Vector3(0, 0, 0);
                print("how the fuck is that possibl *komoji* ??");
                break;
        }

        

        _target.Translate(move * Time.unscaledDeltaTime * _smoothSpeed);

        Vector3 desiredPosition = _target.position + _curOffset * _zoomScale;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.unscaledDeltaTime);
        transform.position = smoothedPosition;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, _curRotation, _smoothSpeed * Time.unscaledDeltaTime);
        transform.rotation = smoothedRotation;
    }

    private void ChangeMode()
    {
        if (_currentMode == LockedMode)
        {
            _currentMode = UnlockedMode;
            _target.SetParent(null, true);
        }
        else
        {
            _target.SetParent(GI._PlayerFetcher().transform, true);
            _currentMode = LockedMode;
            _target.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void RotateToLeft(InputAction.CallbackContext context)
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

    public void RotateToRight(InputAction.CallbackContext context)
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
