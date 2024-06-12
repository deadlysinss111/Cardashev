using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MapBlocker : MonoBehaviour
{
    /*
     FIELDS
    */
    // public allows for setting at instantiation
    public float _secBeforeClose;
    float _minimDist;
    bool _isLocked;

    [SerializeField] GameObject _door1;
    [SerializeField] GameObject _door2;


    /*
     PROPERTIES
    */
    public bool _IsLocked { get => _isLocked; }


    /*
     METHODS
    */
    private void Awake()
    {
        _isLocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale;
        // Oh nyo... Nested ifs (based) (feur)
        if (_isLocked)
        {
            // Updates the space between the 2 door's panes
            if (_door1.transform.localScale.x < 1)
            {
                scale = _door1.transform.localScale;
                scale.x += 0.6f * Time.deltaTime;

                _door1.transform.localScale = scale;
                _door2.transform.localScale = scale;
            }
            return;
        }

        // Puts the target scale to a fraction between fully open and fully closed
        scale = new(Mathf.Clamp01(GI._gameTimer / _secBeforeClose) * _minimDist, 1, 1);
        _door1.transform.localScale = scale;
        _door2.transform.localScale = scale;

        if (scale.x >= _minimDist)
        {
            _isLocked = true;
        }
    }
}
