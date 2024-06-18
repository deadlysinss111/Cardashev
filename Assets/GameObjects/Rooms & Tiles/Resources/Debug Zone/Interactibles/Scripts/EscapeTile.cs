using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeTile : Interactible
{
    // Contains name of the condition (such as enemy name) and amount of time this condition must be resolved to clear the room
    [SerializeField] Dictionary<string, byte> _conditions = new();

    Action OnWalk = () => { print("locked"); };

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            OnWalk();
    }

    // You can either add a condition trough code, or set conditions by SerializedField
    public void AddCondition(string name, byte amount = 1)
    {
        if (_conditions.ContainsKey(name))
            _conditions[name] += amount;
        else
            _conditions.Add(name, amount);
    }

    // We must call this function whene a condition triggers (such as enemy death)
    public void TriggerCondition(string name)
    {
        if(name != null)
        {
            if (_conditions.ContainsKey(name))
            {
                _conditions[(name)] -= 1;
            }
        }
        foreach (byte amount in _conditions.Values)
        {
            if (amount > 0)
                return;
        }

        LevelCleared();
    }

    // Whene all conditions are resolved
    void LevelCleared()
    {
        // TODO: change sprite or smth
        // & trigger rewards
        Reward._content = new Reward.Content(10, 20, 0);
        OnWalk = ()=> { GI._loader.LoadScene("Reward", "Reward"); };
    }
}
