using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UltimateManager : MonoBehaviour
{
    public Image _cooldownSprite;
    public float _cooldownTime;
    bool _isInCooldown;

    [SerializeField] PlayerInput _pInput;

    private void Awake()
    {
        _pInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _pInput.actions["Ultimate"].performed += OnUltimatePerformed;
    }

    private void OnDisable()
    {
        _pInput.actions["Ultimate"].performed -= OnUltimatePerformed;
    }

    private void Update()
    {
        if (_isInCooldown)
        {

            _cooldownSprite.fillAmount += 1 / _cooldownTime * Time.deltaTime;
            if (_cooldownSprite.fillAmount >= 1)
            {
                _cooldownSprite.fillAmount = 0;
                _isInCooldown = false;
            }
        }
    }

    private void OnUltimatePerformed(InputAction.CallbackContext context)
    {
        UseUltimate();
    }

    public void UseUltimate()
    {
        if (!_isInCooldown)
        {
            Debug.Log("Ultimate used!");
            _isInCooldown = true;
            _cooldownSprite.fillAmount = 0;
        }
    }
}
