using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    int _baseHealth;
    float _baseMoveSpeed;
    float _baseAttack;

    public int _health;
    public float _moveSpeed;
    public float _attack;
    public int _armor;

    bool _wasJustModified;

    List<Modifier> _modifiers;
    PlayerManager _manager;

    private void Awake()
    {
        _manager = GetComponent<PlayerManager>();
        _modifiers = new List<Modifier>();
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
                // TODO: Look at it with Arthus
                case Modifier.ModifierType.Health:
                    _health += (int)math.floor(_baseHealth * mod._value);
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
}
