using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHandler : MonoBehaviour
{
    StatManager _stats;
    bool _waitForDestroy;

    public float Health { get => _stats._health; private set => throw new InvalidCastException(); }

    ParticleSystem _particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        _stats = GetComponent<StatManager>();
        _stats._health = 69;

        _particleSystem = GetComponent<ParticleSystem>();

        _waitForDestroy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_waitForDestroy == false)
        { 
            if (Input.GetKeyDown(KeyCode.H))
            {
                _stats._health -= 10;
            }
            if (_stats._health <= 0)
            {
                Defeat();
            }
            return;
        }

        if (_particleSystem.isEmitting == false)
        {
            Color c = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            c.a = c.a - (1.0f * Time.deltaTime);
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
        }
        if (_particleSystem.isStopped)
        {
            Destroy(gameObject);
        }
    }

    private void Defeat()
    {
        _particleSystem.Play();
        _waitForDestroy = true;
    }
}
