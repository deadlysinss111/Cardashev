using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [NonSerialized] public StatManager _health;

    private void Awake()
    {
        _health = GetComponent<StatManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
