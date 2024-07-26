using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameObject _healthBar;

    float _MaxHealth;
    Vector3 _currentHealth;
    float _scale;

    StatManager _statManager;

    Color _defaultColor;

    void Start()
    {
        _healthBar = GameObject.Find("HealthBar");
        GameObject player = GameObject.Find("Player");
        _statManager = player.GetComponent<StatManager>();
        _MaxHealth = _statManager._baseHealth;

        _defaultColor = _healthBar.GetComponent<Slider>().fillRect.gameObject.GetComponent<Image>().color;
    }

    void Update()
    {
        if (_statManager != null)
        {
            // Temp so I can actually see if critical is on or not
            if (_statManager.HasCritical())
                _healthBar.GetComponent<Slider>().fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
            else
                _healthBar.GetComponent<Slider>().fillRect.gameObject.GetComponent<Image>().color = _defaultColor;

            _healthBar.GetComponent<Slider>().value = _statManager.Health / _MaxHealth;
            //_currentHealth = _healthBar.transform.localScale;
            //_currentHealth.x = (_statManager.Health / _MaxHealth);
            //_healthBar.transform.localScale = _currentHealth;
        }
    }
}
