using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SlowMotionWithProgressBar : MonoBehaviour
{
    // Slow motion variables
    public float _slowdownFactor;
    public float _slowdownLength; // Maximum duration the slow motion can last

    // Focus bar variables
    public GameObject _focusBar;

    private bool _isActive;
    private PlayerInput _pInput; // Changed from CustomActions to PlayerInput

    // Circular progress bar variables
    private bool _progressBarIsActive;
    private bool _isRefilling;
    private float _indicatorTimer;
    private float _maxIndicatorTimer;
    private Image _radialProgressBar;
    private float _lerpSpeed = 3f; // Speed of interpolation

    private void Awake()
    {
        _pInput = GetComponent<PlayerInput>(); // Ensure PlayerInput component is attached

        // Initialize the radial progress bar
        _radialProgressBar = _focusBar.transform.Find("RadialProgressBar").GetComponent<Image>();
    }

    private void OnEnable()
    {
        // Enable the input actions when the object is enabled
        _pInput.actions["Focus"].performed += OnFocusPerformed;
        _pInput.actions["Focus"].canceled += OnFocusCanceled;
        _pInput.actions["Focus"].Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions when the object is disabled
        _pInput.actions["Focus"].performed -= OnFocusPerformed;
        _pInput.actions["Focus"].canceled -= OnFocusCanceled;
        _pInput.actions["Focus"].Disable();
    }

    private void OnFocusPerformed(InputAction.CallbackContext context)
    {
        StartSlowMotion();
    }

    private void OnFocusCanceled(InputAction.CallbackContext context)
    {
        StopSlowMotion();
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
                
                _indicatorTimer += Time.unscaledDeltaTime / 4; 
                _radialProgressBar.fillAmount = Mathf.Lerp(_radialProgressBar.fillAmount, _indicatorTimer / _maxIndicatorTimer, _lerpSpeed * Time.unscaledDeltaTime);

                IncreaseTimer(); 

                
                if (_indicatorTimer >= _maxIndicatorTimer)
                {
                    _isRefilling = false;
                    _indicatorTimer = _maxIndicatorTimer; 
                    _radialProgressBar.fillAmount = 1f; 
                    _slowdownLength = _maxIndicatorTimer; 

                    
                    StopCountdown();
                }
            }
            else
            {
                _indicatorTimer -= Time.unscaledDeltaTime; 
                _radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;

                if (_indicatorTimer <= 0)
                {
                    StopCountdown();
                }
            }
        }
    }
}