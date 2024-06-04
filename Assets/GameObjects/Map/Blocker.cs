using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private float _limit;

    private bool _isLocked;
    bool IsLocked { get { return _isLocked; } }

    public GameObject _door1;
    public GameObject _door2;

    private void Awake()
    {
        _isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale;
        // Oh nyo... Nested ifs
        if (_isLocked)
        {
            if (_door1.transform.localScale.x < 1)
            {
                scale = _door1.transform.localScale;
                scale.x += 0.6f*Time.deltaTime;

                _door1.transform.localScale = scale;
                _door2.transform.localScale = scale;
            }
            return;
        }

        scale = new(Mathf.Clamp01(GlobalInformations._gameTimer / _time)*_limit, 1, 1);

        _door1.transform.localScale = scale;
        _door2.transform.localScale = scale;
        if (scale.x >= _limit)
        {
            _isLocked = true;
        }
    }

    private void FixedUpdate()
    {
        //GlobalInformations._gameTimer += Time.deltaTime;
    }
}
