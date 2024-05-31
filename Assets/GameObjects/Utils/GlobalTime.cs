using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{
    // Time in milliseconds, should be used for almost everything related to "real time"
    private float _globalTime = 0;

    private void FixedUpdate()
    {
        //BetterDebug.Log(_globalTime);
        _globalTime += 1f;
    }

    public float getTime()
    {
        return _globalTime;
    }

    public float getSeconds()
    {
        return getTime() / 1000;
    }
}
