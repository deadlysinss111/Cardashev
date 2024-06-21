using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ultimate : MonoBehaviour
{
    /*
     FIELDS
    */
    // Ultimate gauge "resource"
    [SerializeField] protected float _ultiEnergyMax;
    protected float _ultiEnergyCur;

    // Energy change rates
    [SerializeField] protected float _regenRate;
    [SerializeField] protected float _depleteRate;

    // Status booleans
    bool _isInEffect;       // Controls if the player is using the effect of their ultimate
    bool _isCooling;        // Controls if the player is gaining Energy back or not


    /*
     PROPERTIES
    */
    // Special case to stop the cooldown when a constant effect is started
    public bool _IsCooling {
        get => _isCooling;
        set
        {
            // TODO: Test if nothing is preventing to go into cooldown
            if (true)
                _isCooling = value;
        }}

    public bool _IsInEffect {
        get => _isInEffect;
        set
        {
            _isInEffect = value;
            _IsCooling = !value;
        }}


    /*
     METHODS
    */
    protected void Awake()
    {
        // Field setup
        _ultiEnergyCur = 0;
        _isInEffect = false;
        _isCooling = true;
    }


    // ------
    // THE 3 MUSKETEERS
    // ------
    protected abstract void StartEffect();
    protected abstract void UpdateEffect();
    protected abstract void StopEffect();


    // ------
    // ENERGY MANAGERS
    // ------
    // Cooldown coroutine caller & its coroutine
    protected void StartCooldown(float ARGregenRate) { StartCoroutine(CoolingDown(ARGregenRate)); } // Doesn't set _isCooling in case we want to prevent is elsewhere
    protected IEnumerator CoolingDown(float ARGregenRate)
    {
        while (_ultiEnergyCur < _ultiEnergyMax && _isCooling)
        {
            _ultiEnergyCur += ARGregenRate * Time.deltaTime;
            yield return null;
        }
    }

    // Energy depletion couroutine caller & its coroutine
    protected void ActivateEffect(float ARGdepleteRate) { _isInEffect = true; StartCoroutine(DepletingEnergy(ARGdepleteRate)); }
    protected IEnumerator DepletingEnergy(float ARGdepleteRate)
    {
        while (_ultiEnergyCur > 0 && _isInEffect)
        {
            _ultiEnergyCur -= ARGdepleteRate * Time.deltaTime;
            yield return null;
        }
    }

    // Segmented energy voider. Also returns whether or not it succeeded
    protected bool? EmptyEnergy(float ARGpercent)
    {
        // Check if the ARG was within valid range
        if (ARGpercent < 0.0f || ARGpercent > 1.0f)
        {
            Debug.LogError("Fucker, this is supposed to be a percentage ! >:(");
            return null;
        }

        // Check if depleting this percentage of energy is possible
        if (ARGpercent * _ultiEnergyMax > _ultiEnergyCur)
            return false;

        // The deplete is possible, so we do that
        _ultiEnergyCur -= ARGpercent * _ultiEnergyMax;
        return true;
    }

}
