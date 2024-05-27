using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowMotion : MonoBehaviour
{
    // Slow motion variables
    public float _slowdownFactor = 0.10f;

    public float _slowdownLength = 2f; // How long the slow motion will last

    // Focus bar variables
    public GameObject _focusBar;

    public bool _isRunning;
    public float _duration = 5f; // Set a default duration

    private Coroutine _slowMotionCoroutine;
    private Coroutine _countdownCoroutine;
    private Coroutine _cooldownCoroutine;

    private CustomActions _input;

    // function to handle the focus action
    private void Awake()
    {
        _input = new CustomActions();
        _input.Main.Focus.performed += OnFocusPerformed;
        _input.Main.Focus.canceled += OnFocusCanceled;
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

    // Handle the focus action
    private void OnFocusPerformed(InputAction.CallbackContext context)// CallbackContext is the context of the action
    {
        DoSlowMotion();
    }

    // Handle the focus action cancelation
    private void OnFocusCanceled(InputAction.CallbackContext context)
    {
        ResetTimeScale();
        StopCountdown();
    }

    // Function to handle the slow motion
    public void DoSlowMotion()
    {
        if (_isRunning || (_cooldownCoroutine != null))
        {
            return;
        }

        if (_slowMotionCoroutine != null)
        {
            StopCoroutine(_slowMotionCoroutine);
        }
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
        }

        Time.timeScale = _slowdownFactor; // Time.timeScale is the speed of the game
        Time.fixedDeltaTime = Time.timeScale * 0.02f; // Time.fixedDeltaTime is the time between each physics update

        _slowMotionCoroutine = StartCoroutine(StopSlowMotion(_duration));// variable to store the coroutine
        _countdownCoroutine = StartCoroutine(StartCountdown(_duration));
    }

    // Function to stop the slow motion
    private IEnumerator StopSlowMotion(float duration)
    {
        // Calculate the time used during slow motion
        float usedTime = _duration - duration;

        yield return new WaitForSecondsRealtime(_slowdownLength); // Use WaitForSecondsRealtime for time scale-independent waiting

        ResetTimeScale();

        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
        }

        _cooldownCoroutine = StartCoroutine(Cooldown(usedTime));
    }

    private IEnumerator Cooldown(float usedTime)
    {
        // Calculate the cooldown duration based on the time used
        float cooldownDuration = usedTime * 2;

        yield return new WaitForSecondsRealtime(cooldownDuration);

        _isRunning = false;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f; // Reset the time scale
        Time.fixedDeltaTime = 0.02f; // Reset the fixed delta time
    }

    private IEnumerator StartCountdown(float duration)
    {
        _isRunning = true;
        _focusBar.SetActive(true);

        var radialProgressBar = _focusBar.transform.Find("RadialProgressBar").GetComponent<CircularProgressBar>();
        if (radialProgressBar != null)
        {
            radialProgressBar.ActivateCountdown(duration);
        }

        yield return new WaitForSecondsRealtime(duration); // Use WaitForSecondsRealtime for time scale-independent waiting

        _isRunning = false;
        _focusBar.SetActive(false);
    }

    private void StopCountdown()
    {
        _isRunning = false;
        _focusBar.SetActive(false);
    }
}