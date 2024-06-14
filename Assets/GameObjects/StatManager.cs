using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    /*  ---> INTERNAL CLASS <---  */
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

    /*
     FIELDS
    */
    int _baseHealth;
    float _baseMoveSpeed;
    float _baseAttack;

    public int _health;
    public float _moveSpeed;
    public float _attack;
    public int _armor;

    bool _wasJustModified;

    List<Modifier> _modifiers;
    [SerializeField] CriticalBar _criticalBar;


    /*
     EVENTS
    */
    UnityEvent _UeDebuffListChange;


    /*
     METHODS
    */
    private void Awake()
    {
        // Event subscribing
        _UeDebuffListChange.AddListener(ApplyModifiers);

        _modifiers = new List<Modifier>();  // ←- should this go into Start() ?
    }

    void Start()
    {
        _health = 100;
        _baseHealth = _health;
        _moveSpeed = 1.5f;
        _baseMoveSpeed = _moveSpeed;
        _attack = 1;
        _baseAttack = _attack;
        _wasJustModified = false;
    }

    void Update()
    {
        // Updates the timer of each stat modifiers that are currently being applied
        foreach (Modifier debuff in _modifiers)
        {
            debuff._duration -= Time.deltaTime;

            if (debuff._duration <= 0)
            {
                _modifiers.Remove(debuff);
                _wasJustModified = true;
                _UeDebuffListChange.Invoke();
            }
        }
    }


    public void AddModifier(Modifier modifier)
    {
        if (modifier == null) return;
        if (modifier._type == Modifier.ModifierType.Critical && HasCritical())
        {
            Modifier mod = GetCriticalModifier();
            _modifiers.Remove(mod);
        }

        _modifiers.Add(modifier);
        _wasJustModified = true;
        _UeDebuffListChange.Invoke();
    }

    // Calculate every modifiers again
    // Should be renamed ReapplyModifiers, ModifierCalculator, ApplyModifierRAZ ?
    void ApplyModifiers()
    {
        //if (!_wasJustModified) return;

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
                // TODO: Look at it with Arthus
                case Modifier.ModifierType.Health:
                    _health += (int)math.floor(_baseHealth * mod._value);
                    break;
                case Modifier.ModifierType.Critical:
                    _attack += _baseAttack * mod._value;
                    _moveSpeed += _baseMoveSpeed * mod._value;
                    _criticalBar.ActivateBuff(mod._duration);
                    break;
            }
        }

        _wasJustModified = false;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;

        if( _health <= 0)
        {
            Enemy enemy;
            if (gameObject.TryGetComponent<Enemy>(out enemy))
            {
                enemy._UeOnDefeat.Invoke();
                return;
            }
            PlayerManager manager;
            if (gameObject.TryGetComponent<PlayerManager>(out manager))
                manager._UeOnDefeat.Invoke();
        }
    }

    public void Heal(int amount)
    {
        _health = math.clamp(_health + amount, 0, _baseHealth);
    }

    public bool HasCritical()
    {
        return GetCriticalModifier() is not null;
    }
    public bool HasCritical(out Modifier critMod)
    {
        critMod = GetCriticalModifier();
        return critMod is not null;
    }

    Modifier GetCriticalModifier()
    {
        foreach (Modifier mod in _modifiers)
        {
            if (mod._type == Modifier.ModifierType.Critical)
                return mod;
        }
        return null;
    }
}
