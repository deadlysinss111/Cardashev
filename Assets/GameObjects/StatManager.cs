using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public class Modifier
    {
        public enum ModifierType
        {
            Speed,
            Attack,
            Health,
            Critical
        }

        public ModifierType _type;
        public float _duration;
        public float _value;

        public Modifier(ModifierType type, float value, float duration)
        {
            _type = type;
            _value = Mathf.Clamp(value, 0.1f, 1);
            _duration = duration;
        }
    }

    public float _health;
    private float _baseHealth;
    public float _moveSpeed;
    private float _baseMoveSpeed;
    public float _attack;
    private float _baseAttack;
    public int _armor;

    bool _wasJustModified;

    List<Modifier> _modifiers;
    PlayerManager _manager;
    [SerializeField] CriticalBar _criticalBar;

    private void Awake()
    {
        _manager = GetComponent<PlayerManager>();
        _modifiers = new List<Modifier>();
    }

    void Start()
    {
        _health = 1;
        _baseHealth = _health;
        _moveSpeed = 1.5f;
        _baseMoveSpeed = _moveSpeed;
        _attack = 1;
        _baseAttack = _attack;
        _wasJustModified = false;
    }

    void Update()
    {
        foreach (Modifier debuff in _modifiers)
        {
            debuff._duration -= Time.deltaTime;

            if (debuff._duration <= 0)
            {
                _modifiers.Remove(debuff);
                _wasJustModified = true;
            }
        }

        ApplyModifiers();
    }

    public void AddModifier(Modifier modifier)
    {
        if (modifier == null) return;
        // If there's already a critical buff on, remove it
        if (modifier._type == Modifier.ModifierType.Critical && HasCritical())
        {
            Modifier mod = GetModifier(Modifier.ModifierType.Critical);
            _modifiers.Remove(mod);
        }

        _modifiers.Add(modifier);
        _wasJustModified = true;
    }

    void ApplyModifiers()
    {
        if (!_wasJustModified) return;

        _health = _baseHealth;
        _moveSpeed = _baseMoveSpeed;

        foreach (Modifier mod in _modifiers)
        {
            switch (mod._type)
            {
                case Modifier.ModifierType.Attack:
                    _attack += _baseAttack * mod._value;
                    break;
                case Modifier.ModifierType.Speed:
                    _moveSpeed += _baseMoveSpeed * mod._value;
                    break;
                case Modifier.ModifierType.Health:
                    _health += _baseHealth * mod._value;
                    break;
                case Modifier.ModifierType.Critical:
                    _attack += _baseAttack * mod._value;
                    _moveSpeed += _baseMoveSpeed * mod._value;
                    _health += _baseHealth * mod._value;
                    _criticalBar.ActivateBuff(mod._duration);
                    break;
            }
        }

        _wasJustModified = false;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
    }

    public void Heal(int amount)
    {
        if(_health+amount > _baseHealth)
        {
            _health = _baseHealth;
        }
        else
        {
            _health += amount;
        }
    }

    // Not sure how useful rn that is but it sounds like it could be at some point

    /// <summary>
    /// Returns the first found modifier of type type
    /// </summary>
    /// <param name="type">The type of the modifier to look for</param>
    /// <returns></returns>
    public Modifier GetModifier(Modifier.ModifierType type)
    {
        foreach (Modifier mod in _modifiers)
        {
            if (mod._type == type)
                return mod;
        }
        return null;
    }
    /// <summary>
    /// Returns a list of all modifiers of type type
    /// </summary>
    /// <param name="type">The type of the modifier to look for</param>
    /// <returns></returns>
    public List<Modifier> GetModifiers(Modifier.ModifierType type)
    {
        List<Modifier> mods = new();
        foreach (Modifier mod in _modifiers)
        {
            if (mod._type == type)
                mods.Add(mod);
        }
        return mods;
    }

    public bool HasCritical()
    {
        return GetModifier(Modifier.ModifierType.Critical) is not null;
    }
    public bool HasCritical(out Modifier critMod)
    {
        critMod = GetModifier(Modifier.ModifierType.Critical);
        return critMod is not null;
    }
}
