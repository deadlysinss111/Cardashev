using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _target;

    public float _smoothSpeed = 8f;
    public Vector3 _offset;
    Action _currentMode;
    CustomActions _input;

    private void Awake()
    {
        _currentMode = LockedMode;
        _input = new CustomActions();
        _input.Enable();
        _input.Main.SpaceBar.performed += ctx => ChangeMode();
    }

    void Update()
    {
        _currentMode();
    }

    private void LockedMode()
    {
        Vector3 desiredPosition = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, _target.position.z + _offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private void UnlockedMode()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0.0f, 0.5f, 0.5f));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, -0.5f, -0.5f));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-1.0f, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1.0f, 0.0f, 0.0f));
        }
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
}
