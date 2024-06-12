using UnityEngine;
using UnityEngine.InputSystem;

// Debug class to stop simulation
public class PauseGame : MonoBehaviour
{
    [SerializeField] InputAction _pauseButton;

    [SerializeField]
    private Canvas _canvas;

    private bool _isPaused = false;

    private void OnEnable()
    {
        _pauseButton.Enable();
    }

    private void OnDisable()
    {
        _pauseButton.Disable();
    }

    private void Start()
    {
        _pauseButton.performed += _ => Pause();
    }

    public void Pause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 0;
            _canvas.enabled = true;
        }
        else
        {
            Time.timeScale = 1;
            _canvas.enabled = false;
        }
    }
}