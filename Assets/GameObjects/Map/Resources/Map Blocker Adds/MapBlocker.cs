using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MapBlocker : DynamicMapObj
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
    private new void Awake()
    {
        // Essential event subscribing to update
        base.Awake();

        _isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        // This code is hopefully deprecated by the Coroutine, but idk tf I was doing   -- Naptiste
        /*
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
        */
    }

    // GreenCloud's update on Map load
    protected override void UpdDynamicMapObj()
    {
        Debug.Log("Animating doors via Coroutine !\nThis is highly very fucking untested so go screal at me if it's broken (Naptiste)");
        StartCoroutine(AnimateDoors());
    }

    IEnumerator AnimateDoors()
    {
        float targetScaleX;

        // Check if the door should be now closed or not, and set an interval to reach for the animation
        if (GI._gameTimer >= _secBeforeClose)
        {
            _isLocked = true;
            targetScaleX = 1.0f;
        }
        else
            targetScaleX = Mathf.Clamp01(GI._gameTimer / _secBeforeClose) * _minimDist;

        // Setting up some values for that dumbass localScale.x restriction
        float scaleDifference = targetScaleX - _door1.transform.localScale.x;
        Vector3 vector3Difference = Vector3.zero;

        // Animate the doors through their scaling
        while (_door1.transform.localScale.x <= targetScaleX)
        {
            // Updating intermediate scaling vector3
            vector3Difference.x += scaleDifference / 100.0f;

            _door1.transform.localScale = vector3Difference;
            _door2.transform.localScale = vector3Difference;

            yield return null;
        }
    }
}
