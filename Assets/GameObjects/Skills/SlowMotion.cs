using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SlowMotionWithProgressBar : MonoBehaviour
{
    // Slow motion Fields
    [SerializeField] float _slowdownFactor;

    Volume _slowdownEffect;

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
        _slowdownEffect = GameObject.Find("TimeStopEffect").GetComponent<Volume>();

        // Initialize the radial progress bar
        //_radialProgressBar = _focusBar.transform.Find("RadialProgressBar").GetComponent<Image>();
    }

    private void Update()
    {
        if (_progressBarIsActive)
        {
            if (_isRefilling)
            {

                _indicatorTimer += Time.unscaledDeltaTime / 4;
                //_radialProgressBar.fillAmount = Mathf.Lerp(_radialProgressBar.fillAmount, _indicatorTimer / _maxIndicatorTimer, _lerpSpeed * Time.unscaledDeltaTime);


                if (_indicatorTimer >= _maxIndicatorTimer)
                {
                    _isRefilling = false;
                    _indicatorTimer = _maxIndicatorTimer;
                    //_radialProgressBar.fillAmount = 1f;


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

        _slowdownEffect.weight = 1f;

        //_focusBar.SetActive(true);
    }

    public void StopSlowMotion()
    {
        _isActive = false;

        // Reset TimeScale
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        _slowdownEffect.weight = 0f;

        StartRefill();
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
