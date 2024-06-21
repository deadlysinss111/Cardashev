using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : Ultimate
{
    /*
     FIELDS
    */
    // FS = Factory Reset, useful to quickly change values in the Unity Editor
    // Could also serve as local default values in case we had a buff/debuff on any of this
    [SerializeField] int _slowFactorFS;
    [SerializeField] int _focusLossThresholdFS;

    // Dynamic, useful to set their values in StartEffect() to pass to UpdateEffect(), and for UpdateEffect() to tweak them
    int _slowFactor;
    [Range(0.0f, 1.0f)] float _focusLossThreshold;


    /*
     METHODS
    */
    private new void Awake()
    {
        // Base Awake()
        base.Awake();

        // Field setup
        _slowFactor = _slowFactorFS;
        _focusLossThreshold = _focusLossThresholdFS;
    }

    // ------
    // THE 3 MUSKETEERS
    // ------
    protected void StartEffect()
    {
        // Set the game-speed of Unity      /!\ NOT SUSTAINABLE, AS THIS COULD ALSO SLOW THINGS WE WOULD LIKE TO KEEP THE SPEED OF (ANIMATIONS, MUSIC, UI, ETC...)
        Time.timeScale = _slowFactor;

        // Stops cooldown and start usage
        _IsInEffect = true;
        ActivateEffect(_depleteRate);
    }

    protected abstract void UpdateEffect()
    {
        // Test if the slow-down effect should start to wither
        if (_ultiEnergyCur / _ultiEnergyMax < _focusLossThreshold)
            _slowFactor += (1.0f - _slowFactor) / ( (/*Requires to know how much more time the deplete would take --> Might need FixedUpdate()*/) / Time.deltaTime);
    }

    protected abstract void StopEffect();
}
