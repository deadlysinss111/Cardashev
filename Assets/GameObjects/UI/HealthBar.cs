using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Image _healthBar;

    float _MaxHealth;
    Vector3 _currentHealth;
    float _scale;

    StatManager _statManager;

    void Start()
    {
        _healthBar = transform.Find("Foreground")?.GetComponent<Image>();
        _scale = _healthBar.transform.localScale.x;
        GameObject player = GameObject.Find("Player");
        _statManager = player.GetComponent<StatManager>();
        _MaxHealth = _statManager.Health;
    }

    void Update()
    {
        if (_statManager != null)
        {
            _currentHealth = _healthBar.transform.localScale;
            _currentHealth.x = (_statManager.Health / _MaxHealth) * _scale;
            _healthBar.transform.localScale = _currentHealth;
        }
    }
}
