using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    public int _health;
    float _timer;

    bool _activated = false;
    bool _timerActive = false;

    [SerializeField] GameObject _cover;

    // Start is called before the first frame update
    void Start()
    {
        _health = -1;
        _timer = -1f;

        _cover.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated == false) return;

        if (_timerActive)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                DisableCover();
            }
        }

        if (_health < 0)
        {
            DisableCover();
            return;
        }
    }

    public void EnableCover(int health, Quaternion rotation, float time = -1f)
    {
        _health = health;
        _timer = time;
        _cover.transform.rotation = rotation;
        _activated = true;

        if (_timer > 0)
        {
            _timerActive = true;
        }
        _cover.SetActive(true);
    }

    public void DisableCover()
    {
        _activated = false;
        _timerActive = false;
        _cover.SetActive(false);
        GI._PlayerFetcher().GetComponent<PlayerController>()._moveMult = 1f;
    }
}
