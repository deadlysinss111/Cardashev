using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBar : MonoBehaviour
{
    private Slider _armorBar;

    private StatManager _playerStats;

    // Start is called before the first frame update
    void Start()
    {
        _armorBar = GetComponent<Slider>();
        _playerStats = GI._PStatFetcher();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerStats.HasArmor())
        {
            _armorBar.maxValue = _playerStats._maxArmor;
            _armorBar.value = _playerStats._armor;
        }
        else
        {
            _armorBar.value = 0;
        }
    }
}
