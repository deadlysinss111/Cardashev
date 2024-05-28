using UnityEngine;
using UnityEngine.UI;

public class SlowMotionWithProgressBar : MonoBehaviour
{
    // Slow motion variables
    public float _slowdownFactor;

    public float _slowdownLength; // Maximum duration the slow motion can last

    // Focus bar variables
    public GameObject _focusBar;

    private bool _isActive;
    private CustomActions _input;

    // Circular progress bar variables
    private bool _progressBarIsActive;

    private bool _isRefilling;
    private float _indicatorTimer;
    private float _maxIndicatorTimer;
    private Image _radialProgressBar;
    private float _lerpSpeed = 3f; // Speed of interpolation

    private void Awake()
    {
        _input = new CustomActions();
        _input.Main.Focus.performed += context => StartSlowMotion();
        _input.Main.Focus.canceled += context => StopSlowMotion();

        // Initialize the radial progress bar
        _radialProgressBar = _focusBar.transform.Find("RadialProgressBar").GetComponent<Image>();
    }

    // Enable and disable the input actions
    private void OnEnable()
    {
        _input.Main.Enable();
    }

    private void OnDisable()
    {
        _input.Main.Disable();
    }

    public void StartSlowMotion()
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        _isActive = true;
        _progressBarIsActive = true;
        _isRefilling = false;

        _focusBar.SetActive(true);
        ActivateCountdown(_slowdownLength);
    }

    public void StopSlowMotion()
    {
        _isActive = false;

        // Reset TimeScale
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        StartRefill();
    }

    public void DecreaseTimer()
    {
        _slowdownLength -= Time.unscaledDeltaTime;
    }

    public void IncreaseTimer()
    {
        _slowdownLength += Time.unscaledDeltaTime / 4;
    }

    public void StartRefill()
    {
        _isRefilling = true;
        _progressBarIsActive = true;
    }

    public void ActivateCountdown(float countdownTime)
    {
        _maxIndicatorTimer = 5f;
        _indicatorTimer = countdownTime;
    }

    public void StopCountdown()
    {
        _progressBarIsActive = false;
    }

    private void Update()
    {
        if (_isActive)
        {
            DecreaseTimer();
            if (_slowdownLength <= 0)
            {
                StopSlowMotion();
            }
        }

        if (_progressBarIsActive)
        {
            if (_isRefilling)
            {
                // Smoothly refill the progress bar using Lerp
                _indicatorTimer += Time.unscaledDeltaTime/4; // Increment the indicator timer
                _radialProgressBar.fillAmount = Mathf.Lerp(_radialProgressBar.fillAmount, _indicatorTimer / _maxIndicatorTimer, _lerpSpeed * Time.unscaledDeltaTime);
                
                IncreaseTimer(); // Increase the timer

                // Stop refilling once max is reached
                if (_indicatorTimer >= _maxIndicatorTimer)
                {
                    _isRefilling = false;
                    _indicatorTimer = _maxIndicatorTimer; // Ensure the timer is exactly at max
                    _radialProgressBar.fillAmount = 1f; // Ensure the fill amount is exactly 1
                    _slowdownLength = _maxIndicatorTimer; // Ensure the timer is exactly at max


                    // Stop the countdown when fully refilled
                    StopCountdown();
                }
            }
            else
            {
                _indicatorTimer -= Time.unscaledDeltaTime; // Use unscaledDeltaTime instead of deltaTime
                _radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;

                if (_indicatorTimer <= 0)
                {
                    StopCountdown();
                }
            }
        }
    }
}