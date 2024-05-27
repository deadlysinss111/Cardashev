using UnityEngine;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    private bool _isActive;

    private float _indicatorTimer;
    private float _maxIndicatorTimer;

    private Image _radialProgressBar;

    private void Awake()
    {
        _radialProgressBar = GetComponent<Image>();
    }

    private void Update()
    {
        if (_isActive)
        {
            _indicatorTimer -= Time.unscaledDeltaTime; // Use unscaledDeltaTime instead of deltaTime
            _radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;

            if (_indicatorTimer <= 0)
            {
                StopCountdown();
            }
        }
    }

    public void ActivateCountdown(float countdownTime)
    {
        _isActive = true;
        _maxIndicatorTimer = countdownTime;
        _indicatorTimer = _maxIndicatorTimer;
    }

    public void StopCountdown()
    {
        _isActive = false;
    }
}