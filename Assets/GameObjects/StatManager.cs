using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

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
            Critical,
            Armor
        }

        public ModifierType _type;
        public float _duration;
        public float _value;

        public Modifier(ModifierType type, float value, float duration=1f)
        {
            _type = type;
            if (type == ModifierType.Armor)
            {
                _value = value;
            }
            else
            {
                _value = Mathf.Clamp(value, 0.1f, 1);
                _duration = duration;
            }
        }
    }

    /*
     FIELDS
    */
    int _baseHealth;
    float _baseMoveSpeed;
    float _baseAttack;

    private int _health; // now read-only to force everyone to use TakeDamage()
    public float _moveSpeed;
    public float _attack;
    [NonSerialized] public int _armor;
    [NonSerialized] public int _maxArmor;

    bool _wasJustModified;
    [SerializeField] OutlineEffectScript _takeDamageEffect;

    List<Modifier> _modifiers;
    [SerializeField] CriticalBar _criticalBar;

    public int Health { get { return _health; } }
    public int RealHealth { get { return _health+_armor; } }
    public int BaseHealth { get { return _baseHealth; } }


    /*
     EVENTS
    */
    UnityEvent _UeDebuffListChange;


    /*
     METHODS
    */
    public Type _type;

    static Type[] _typeList = { Type.GetType("Ebouillantueur"), Type.GetType("Murlock") };

    public GameOverManager _gameOverManager;

    private void Awake()
    {
        // Event subscribing
        _UeDebuffListChange = new();
        _UeDebuffListChange.AddListener(ApplyModifiers);

        _modifiers = new List<Modifier>();  // ←- should this go into Start() ?
    }

    void Start()
    {
        if (gameObject.TryGetComponent<PlayerManager>(out PlayerManager manager))
            _health = Idealist._instance._baseHP;
        else if (gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            _health = enemy._health;
        else if (gameObject.TryGetComponent<Interactible>(out Interactible interactible))
            _health = interactible._health;
        else
            throw new Exception("are you trying to have a stat manager on a non player / enemy / interactible object?");

        _baseHealth = _health;
        _moveSpeed = 1.5f;
        _baseMoveSpeed = _moveSpeed;
        _attack = 1;
        _baseAttack = _attack;
        _armor = -1;
        _maxArmor = _armor;
        _wasJustModified = false;
    }

    void Update()
    {
        // Updates the timer of each stat modifiers that are currently being applied
        for (int i = _modifiers.Count-1; i >= 0; i--)
        {
            //print(i);
            Modifier debuff = _modifiers[i];
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
        // If there's already a critical buff on, remove it
        if (modifier._type == Modifier.ModifierType.Critical && HasCritical(out Modifier mod))
        {
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

        //_health = _baseHealth;
        _moveSpeed = _baseMoveSpeed;

        for (int i = _modifiers.Count-1; i >= 0; i--)
        {
            print(i);
            Modifier mod = _modifiers[i];
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
                    //_criticalBar.ActivateBuff(mod._duration);
                    break;
                case Modifier.ModifierType.Armor:
                    _armor = (int)mod._value;
                    _maxArmor = _armor;
                    _modifiers.Remove(mod); // No need to keep it any longer once the value is set
                    break;
            }
        }

        _wasJustModified = false;
    }

    public void TakeDamage(int amount, bool ignore_armor=false)
    {
        if (HasArmor() && ignore_armor == false)
        {
            int diff = _armor - amount;
            _armor = diff;
            if (diff >= 0)
                return;
            amount -= Math.Abs(diff);
            
        }

        _health -= amount;
        if (_takeDamageEffect) _takeDamageEffect.TakeDamageEffect();

        if ( _health <= 0)
        {
            if (gameObject.TryGetComponent(out Enemy enemy))
            {
                //enemy._UeOnDefeat.Invoke();
                //var ratilo = enemy.GetType().GetField("Defeat", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                //print("ratilo there : "+ enemy.GetType() );
                enemy.Defeat();
                return;
            }
            if (gameObject.TryGetComponent(out Interactible interactible))
            {
                interactible.Kill();
                return;
            }
            if (gameObject.TryGetComponent(out PlayerManager player))
            {
                player._UeOnDefeat.Invoke();
                _gameOverManager.StartGameOver();
            }
        }
    }

    public void Heal(int amount)
    {
        _health = math.clamp(_health + amount, 0, _baseHealth);
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

    public bool HasArmor()
    {
        return _armor > 0;
    }
}
