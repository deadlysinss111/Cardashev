using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UltimateManager : MonoBehaviour
{
    public Image _cooldown;
    public float _cooldownTime;
    private bool _isCooldown;

    [SerializeField]
    private PlayerInput _pInput;

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

    private void OnUltimatePerformed(InputAction.CallbackContext context)
    {
        
        UseUltimate();
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
