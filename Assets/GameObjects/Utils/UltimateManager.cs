using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UltimateManager : MonoBehaviour
{
    public Image _cooldown;
    public float _cooldownTime;
    bool _isCooldown;

    CustomActions _controls;

    private void Awake()
    {
        _controls = new CustomActions();
        _controls.Main.Ultimate.performed += ctx => UseUltimate();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    public void UseUltimate()
    {
        if (!_isCooldown)
        {
            Debug.Log("Ultimate used!");
            _isCooldown = true;
            _cooldown.fillAmount = 0;
        }
    }

    private void Update()
    {
        if (_isCooldown)
        {
            _cooldown.fillAmount += 1 / _cooldownTime * Time.deltaTime;
            if (_cooldown.fillAmount >= 1)
            {
                _cooldown.fillAmount = 0;
                _isCooldown = false;
            }
        }
    }
}
