using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameObject _healthBar;

    float _MaxHealth;
    Vector3 _currentHealth;
    float _scale;

    StatManager _statManager;

    void Start()
    {
        _healthBar = GameObject.Find("HealthBar");
        GameObject player = GameObject.Find("Player");
        _statManager = player.GetComponent<StatManager>();
        _MaxHealth = _statManager.Health;
    }

    void Update()
    {
        if (_statManager != null)
        {
            _healthBar.GetComponent<Slider>().value = _statManager.Health / _MaxHealth;
            //_currentHealth = _healthBar.transform.localScale;
            //_currentHealth.x = (_statManager.Health / _MaxHealth);
            //_healthBar.transform.localScale = _currentHealth;
        }
    }
}
