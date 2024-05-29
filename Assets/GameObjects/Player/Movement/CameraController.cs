using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public Transform _target;

    public float _smoothSpeed = 8f;
    public Vector3 _offset;
    Quaternion _rotation;
    Action _currentMode;
    CustomActions _input;

    private void Awake()
    {
        _currentMode = LockedMode;
        _input = new CustomActions();
        _input.Enable();
        _input.Main.SpaceBar.performed += ctx => ChangeMode();
        _input.CameraControls.RotateToLeft.performed += ctx => RotateToLeft();
        _input.CameraControls.RotateToRight.performed += ctx => RotateToRight();
        _rotation = transform.rotation;
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
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, _rotation, _smoothSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
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
}
