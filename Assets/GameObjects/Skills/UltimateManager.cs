using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class UltimateManager : MonoBehaviour
{
    /*
     FIELDS
    */
    public Image _cooldownSprite;
    public float _cooldownTime;
    bool _isInCooldown;

    [SerializeField] PlayerInput _pInput;


    /*
     METHODS
    */
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
        // Old update, obsolete if the coroutine works
        /*
        if (_isInCooldown)
        {

            _cooldownSprite.fillAmount += 1 / _cooldownTime * Time.deltaTime;
            if (_cooldownSprite.fillAmount >= 1)
            {
                _cooldownSprite.fillAmount = 0;
                _isInCooldown = false;
            }
        }
        */
    }

    // Function linked to the current control for it
    private void OnUltimatePerformed(InputAction.CallbackContext context)
    {
        UseUltimate();
    }

    public void UseUltimate()
    {
        if (!_isInCooldown)
        {
            Debug.Log("Ultimate used!");
            StartCoroutine(UltimateCooldown());     // /!\ This is pretty terrible, since it does not wait for the ultimate to END before initiating cooldown

            // Obsolete (if the coroutine works, that is)
            _isInCooldown = true;
            _cooldownSprite.fillAmount = 0;
        }
    }

    IEnumerator UltimateCooldown()
    {
        bool isFillupDone = false;

        while (!isFillupDone)
        {
            // Fills up the sprite bar by a fraction proportional to the time waited
            _cooldownSprite.fillAmount += 1 / _cooldownTime * Time.deltaTime;

            // Check if the fill-up is over, ending the while loop if it is
            if (_cooldownSprite.fillAmount >= 1)
            {
                _cooldownSprite.fillAmount = 0; // Why does it reset to 0 ???
                isFillupDone = true;
            }

            yield return null;
        }
    }
}
