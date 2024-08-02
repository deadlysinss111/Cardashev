using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
        // Should really be renamed StatusEffects
        public enum ModifierType
        {
            Speed,
            Attack,
            Health,
            Critical,
            Armor,

            // Aren't really "modifiers"
            RadPoison
        }

        public ModifierType _type;
        public float _durationLeft;
        public float _value;

        // Used exclusively for periodical effects for now
        public float? _applyFrequency;
        public int? _applyCounter;

        public Modifier(ModifierType type, float value, float duration = 1f, float? applyFrequency = null)
        {
            _type = type;
            if (type == ModifierType.Armor)
            {
                _value = value;
            }
            else
            {
                _value = Mathf.Clamp(value, 0.1f, 1);
                _durationLeft = duration;
                _applyFrequency = applyFrequency;
                _applyCounter = applyFrequency == null ? null : 0;
            }
        }
    }

    /*
     FIELDS
    */
    public int _baseHealth;
    float _baseMoveSpeed;
    float _baseAttack;

    private int _health; // now read-only to force everyone to use TakeDamage()
    public float _moveSpeed;
    public float _attack;
    [NonSerialized] public int _armor;
    [NonSerialized] public int _maxArmor;

    [SerializeField] OutlineEffectScript _takeDamageEffect;

    // Array for each possible modifiers the player can be affected by AND for how long they have it in a row. Each index SHOULD BE RESERVED for the effects' Enum value (code example in AddModifier)
    public Modifier?[] _statusEffectArr = new Modifier[Enum.GetNames(typeof(StatManager.Modifier.ModifierType)).Length];
    public float[] _statusAgeArr = new float[Enum.GetNames(typeof(StatManager.Modifier.ModifierType)).Length];

    [SerializeField] CriticalBar _criticalBar;
    public GameOverManager _gameOverManager;

    public int Health { get { return _health; } }
    public int RealHealth { get { return _health + _armor; } }

    public Type _type;
    static Type[] _typeList = { Type.GetType("Ebouillantueur"), Type.GetType("Murlock") };


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
        _UeDebuffListChange = new();
        _UeDebuffListChange.AddListener(ApplyModifiers);

        /*_modifiers = new List<Modifier>();  // ←- should this go into Start() ?*/
    }

    void Start()
    {
        PlayerManager manager;
        if (gameObject.TryGetComponent<PlayerManager>(out manager))
            _health = Idealist._instance._baseHP;
        else
            _health = 20;

        _baseHealth = _health;
        _moveSpeed = 1.5f;
        _baseMoveSpeed = _moveSpeed;
        _attack = 1;
        _baseAttack = _attack;
        _armor = -1;
        _maxArmor = _armor;
    }

    void Update()
    {
        // Updates the timer of each stat modifiers that are currently being applied
        for (int i = _statusEffectArr.Length - 1; i >= 0; i--)
        {
            // Fetches pointers
            ref Modifier pStatusEffect = ref _statusEffectArr[i];
            ref float pStatusAge = ref _statusAgeArr[i];

            // Updates duration and time spent having the status effect if they are applied to the player currently
            if (pStatusEffect != null)
            {
                pStatusEffect._durationLeft -= Time.deltaTime;
                pStatusAge += Time.deltaTime;

                if (pStatusEffect._durationLeft <= 0)
                {
                    pStatusEffect = null;
                    _UeDebuffListChange.Invoke();
                }
            }
        }
    }


    // When adding modifiers, please add it using the value of the Enum as the index !
    public void AddModifier(Modifier modifier)
    {
        if (modifier == null) return;

        // If there's already a critical buff on, remove it
        if (modifier._type == Modifier.ModifierType.Critical && HasCritical(out Modifier mod))
            _statusEffectArr[(int)Modifier.ModifierType.Critical] = null;

        Debug.Log("Added " + modifier._type + " (Previously null if smile : " + _statusEffectArr[(int)modifier._type] + ")");

        _statusEffectArr[(int)modifier._type] = modifier;
        Debug.Log("Adding finished ! Index " + (int)modifier._type + " is null if smile tho : " + _statusEffectArr[(int)modifier._type] + ")");

        _UeDebuffListChange.Invoke();
    }

    // Calculate every modifiers again
    // Should be renamed ReapplyModifiers, ModifierCalculator, ApplyModifierRAZ ?
    void ApplyModifiers()
    {
        _moveSpeed = _baseMoveSpeed;

        for (int i = 0; i < _statusEffectArr.Length; ++i)
        {
            // Doens't do anything if the effect isn't there
            if (_statusEffectArr[i] == null) return;

            ref Modifier statusEffect = ref _statusEffectArr[i];
            Debug.Log("Applying " + _statusEffectArr[i]._type + " (" + i + ")");
            switch (statusEffect._type)
            {
                case Modifier.ModifierType.RadPoison:
                    PeriodicalDamageCheck(statusEffect._type);
                    //StartCoroutine(TakeDamagePeriodically(10, 1.0f, ));
                    break;
                case Modifier.ModifierType.Attack:
                    _attack += _baseAttack * statusEffect._value;
                    break;
                case Modifier.ModifierType.Speed:
                    _moveSpeed += _baseMoveSpeed * statusEffect._value;
                    break;
                // TODO: Look at it with Arthus
                case Modifier.ModifierType.Health:
                    _health += (int)math.floor(_baseHealth * statusEffect._value);
                    break;
                case Modifier.ModifierType.Critical:
                    _attack += _baseAttack * statusEffect._value;
                    _moveSpeed += _baseMoveSpeed * statusEffect._value;
                    //_criticalBar.ActivateBuff(mod._duration);
                    break;
                case Modifier.ModifierType.Armor:
                    _armor = (int)statusEffect._value;
                    _maxArmor = _armor;
                    statusEffect = null;
                    break;
            }
        }
    }

    public void TakeDamage(int amount, bool ignore_armor = false)
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

        if (_health <= 0)
        {
            if (gameObject.TryGetComponent(out Enemy enemy))
            {
                //enemy._UeOnDefeat.Invoke();
                var ratilo = enemy.GetType().GetField("Defeat", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                //print("ratilo there : "+ enemy.GetType() );
                enemy.Defeat();
                return;
            }
            if (gameObject.TryGetComponent(out PlayerManager player))
            {
                player._UeOnDefeat.Invoke();
                GameObject.Find("GameOverScreen").GetComponent<GameOverManager>().StartGameOver();
            }
        }
    }

    public void Heal(int amount)
    {
        _health = math.clamp(_health + amount, 0, _baseHealth);
    }

    // Not sure how useful rn that is but it sounds like it could be at some point //DEPREC

    /// <summary>
    /// Returns the first found modifier of type type
    /// </summary>
    /// <param name="type">The type of the modifier to look for</param>
    /// <returns></returns>
    public Modifier GetModifier(Modifier.ModifierType type)
    {
        return _statusEffectArr[(int)type];
    }
    /// <summary>
    /// Returns a list of all modifiers of type type
    /// </summary>
    /// <param name="type">The type of the modifier to look for</param>
    /// <returns></returns>
    public List<Modifier> GetModifiers(Modifier.ModifierType type) //DEPREC
    {
        List<Modifier> mods = new();
        foreach (Modifier mod in _statusEffectArr)
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

    public void PeriodicalDamageCheck(Modifier.ModifierType ARGmodType)
    {
        // Fetches pointers
        ref Modifier pStatusEffect = ref _statusEffectArr[(int)ARGmodType];
        ref float pStatusAge = ref _statusAgeArr[(int)ARGmodType];

        // Test with hard-coded values which amounts of times need to have passed to damage the player
        switch (ARGmodType)
        {
            case Modifier.ModifierType.RadPoison:
                // Finds the amount of time the effect was applied since player got the effect
                int cntAppliedSinceBeginning = Mathf.FloorToInt(pStatusAge / (float)pStatusEffect._applyFrequency);

                // Applies effect as much as needed to be catched up
                if (cntAppliedSinceBeginning > pStatusEffect._applyCounter)
                    while (pStatusEffect._applyCounter < cntAppliedSinceBeginning)
                    {
                        GI._PStatFetcher().TakeDamage(Mathf.FloorToInt(pStatusEffect._value));
                        ++pStatusEffect._applyCounter;
                        Debug.Log("RadPoison was appplied ! This is the " + pStatusEffect._applyCounter + "th time.");
                    }
                break;

            default:
                throw new NotSupportedException("PeriodicalDmgChck was passed an unsupported damaging effect : " + ARGmodType.ToString());
        }
    }

    //IEnumerator TakeDamagePeriodically(Modifier.ModifierType ARGmodType)
    //{
    //    // Fetches pointers
    //    ref Modifier pStatusEffect = ref _statusEffectArr[(int)ARGmodType];
    //    ref float pStatusAge = ref _statusAgeArr[(int)ARGmodType];

    //    while (timeElapsed < _modifiers[ARGmodiferIndex]._duration)
    //    {
    //        timeElapsed += Time.deltaTime;

    //        // Calculate how many times damage should have been applied by now (in case some laggy frames pull up)
    //        int dmgApplicationsTH = Mathf.FloorToInt(timeElapsed / ARGdtFrequency);

    //        // Apply damage as much times as needed
    //        while (dmgApplicationsFR < dmgApplicationsTH)
    //        {
    //            TakeDamage(ARGdmgAmount, ARGignoreArmor);
    //            dmgApplicationsFR++;
    //        }

    //        yield return null;
    //    }
    //}

    /*// Coroutine used to deal periodical damage
    // This cannot be stopped for now
    IEnumerator TakeDamagePeriodically(int ARGdmgAmount, float ARGdtFrequency, int ARGmodiferIndex, bool ARGignoreArmor = false)
    {
        float timeElapsed = 0.0f;
        int dmgApplicationsFR = 0;

        while (timeElapsed < _modifiers[ARGmodiferIndex]._duration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate how many times damage should have been applied by now (in case some laggy frames pull up)
            int dmgApplicationsTH = Mathf.FloorToInt(timeElapsed / ARGdtFrequency);

            // Apply damage as much times as needed
            while (dmgApplicationsFR < dmgApplicationsTH)
            {
                TakeDamage(ARGdmgAmount, ARGignoreArmor);
                dmgApplicationsFR++;
            }

            yield return null;
        }
    }*/
    // Coroutine used to deal periodical damage
    // This cannot be stopped for now

    public void SetModifierDuration(Modifier.ModifierType ARGmodifierType, float ARGvalue)
    {
        //// If the modifier can be found in the list, apply changes to it
        //foreach (Modifier mod in _statusEffectArr)
        //    if (mod._type == ARGmodifierType)
        //    {
        //        mod._durationLeft = ARGvalue;
        //        return;
        //    }

        // Fetches pointers
        ref Modifier target = ref _statusEffectArr[(int)ARGmodifierType];

        // Sets the value if the effect exists
        if (target != null)
            target._durationLeft = ARGvalue;
        else
            Debug.LogError("SetModifierDuration couldn't find the modifier !");

        Debug.Log("duration is now " + target._durationLeft);
    }
}
