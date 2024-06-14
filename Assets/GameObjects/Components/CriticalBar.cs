using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CriticalBar : MonoBehaviour
{
    float _totalTime;
    float _maxTime; // Used for clamping only

    Image _bar;

    // Start is called before the first frame update
    void Start()
    {
        _totalTime = -1;
        _maxTime = _totalTime;

        _bar = GetComponent<Image>();
        _bar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_totalTime < 0) return;

        _totalTime -= Time.deltaTime;
        _bar.fillAmount = Mathf.Clamp01(_totalTime/_maxTime);
    }

    public void ActivateBuff(float time)
    {
        _totalTime = time;
        _maxTime = _totalTime;
    }

    public void StopBuff()
    {
        _totalTime = -1;
        _bar.fillAmount = 0;
    }

    public bool HasBuff()
    {
        return _totalTime > 0;
    }
}
