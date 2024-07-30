using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SlowMotionWithProgressBar : MonoBehaviour
{
    // Slow motion Fields
    [SerializeField] float _slowdownFactor;
    [SerializeField] float _slowdownDuration;   // Maximum duration the slow motion can last

    // Focus bar Fields
    [SerializeField] GameObject _focusBar;

    bool _isActive;
    PlayerInput _pInput;

    // Circular progress bar Fields
    bool _progressBarIsActive;
    bool _isRefilling;
    float _indicatorTimer;
    float _maxIndicatorTimer;
    Image _radialProgressBar;
    float _lerpSpeed = 3f;  // Speed of interpolation

    private void Awake()
    {
        _pInput = GetComponent<PlayerInput>(); // Ensure PlayerInput component is attached

        // Initialize the radial progress bar
        //_radialProgressBar = _focusBar.transform.Find("RadialProgressBar").GetComponent<Image>();
    }

    private void Update()
    {
        if (_isActive)
        {
            DecreaseTimer();
            if (_slowdownDuration <= 0)
            {
                //StopSlowMotion();
            }
        }

        if (_progressBarIsActive)
        {
            if (_isRefilling)
            {

                _indicatorTimer += Time.unscaledDeltaTime / 4;
                //_radialProgressBar.fillAmount = Mathf.Lerp(_radialProgressBar.fillAmount, _indicatorTimer / _maxIndicatorTimer, _lerpSpeed * Time.unscaledDeltaTime);

                IncreaseTimer();


                if (_indicatorTimer >= _maxIndicatorTimer)
                {
                    _isRefilling = false;
                    _indicatorTimer = _maxIndicatorTimer;
                    //_radialProgressBar.fillAmount = 1f;
                    _slowdownDuration = _maxIndicatorTimer;


                    StopCountdown();
                }
            }
            else
            {
                _indicatorTimer -= Time.unscaledDeltaTime;
                //_radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;

                if (_indicatorTimer <= 0)
                {
                    StopCountdown();
                }
            }
        }
    }

    private void OnEnable()
    {
        // Enable the input actions when the object is enabled
        _pInput.actions["Focus"].performed += ToggleFocus;
        _pInput.actions["Focus"].Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions when the object is disabled
        _pInput.actions["Focus"].performed -= ToggleFocus;
        _pInput.actions["Focus"].Disable();
    }
    
    //private void OnEnable()
    //{
    //    // Enable the input actions when the object is enabled
    //    _pInput.actions["Focus"].performed += OnFocusPerformed;
    //    _pInput.actions["Focus"].canceled += OnFocusCanceled;
    //    _pInput.actions["Focus"].Enable();
    //}

    //private void OnDisable()
    //{
    //    // Disable the input actions when the object is disabled
    //    _pInput.actions["Focus"].performed -= OnFocusPerformed;
    //    _pInput.actions["Focus"].canceled -= OnFocusCanceled;
    //    _pInput.actions["Focus"].Disable();
    //}

    private void OnFocusPerformed(InputAction.CallbackContext context)
    {
        StartSlowMotion();
    }

    private void OnFocusCanceled(InputAction.CallbackContext context)
    {
        StopSlowMotion();
    }
    private void ToggleFocus(InputAction.CallbackContext context)
    {
        if (_isActive)
            StopSlowMotion();
        else
            StartSlowMotion();
    }

    public void StartSlowMotion()
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        _isActive = true;
        _progressBarIsActive = true;
        _isRefilling = false;

        //_focusBar.SetActive(true);
        ActivateCountdown(_slowdownDuration);
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
        _slowdownDuration -= Time.unscaledDeltaTime;
    }

    public void IncreaseTimer()
    {
        _slowdownDuration += Time.unscaledDeltaTime / 4;
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
}
