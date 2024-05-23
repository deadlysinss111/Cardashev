using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public StatManager _health;
    [NonSerialized] public Vector3 _virtualPos;

    private void Awake()
    {
        _health = GetComponent<StatManager>();
    }

    // we expect a cool state machine that enable / disable controls affectation :)
}
