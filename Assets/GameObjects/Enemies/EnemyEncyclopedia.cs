using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public struct EnemyCard
{
    public string _name;
    public Func<GameObject, bool> _effect;
    public float _duration;

    internal EnemyCard(string name, Func<GameObject, bool> effect, float duration)
    {
        _name = name;
        _effect = effect;
        _duration = duration;
    }
}

public struct Enemy
{
    public string _name;
    public List<string> _cards;
    public string _personality;

    internal Enemy(string name, List<string> cards, string personality)
    {
        _name = name;
        _cards = cards;
        _personality = personality;
    }
}


static public class EnemyEncyclopedia
{
    static public Dictionary<string, EnemyCard> _enemyCardBook = new Dictionary<string, EnemyCard>()
    {
        { "LaunchGrenade", new EnemyCard("LaunchGrenade", (GameObject target)=>
        {
            UnityEngine.Object GRENADE = Resources.Load("Grenade");
            GameObject grenade = (GameObject)MonoBehaviour.Instantiate(GRENADE);
            Vector3 pos = target.transform.position;
            pos.y += 0.5f;
            grenade.GetComponent<Rigidbody>().transform.position = pos;
            grenade.GetComponent<Rigidbody>().velocity = TrailCalculator.BellCurveInititialVelocity(grenade.GetComponent<Rigidbody>().transform.position, GameObject.Find("Player").transform.position, 5);
            return true;
        }, 2) }
    };

    static public Dictionary<string, List<string>> _enemyBook = new Dictionary<string, List<string>>()
    {
        {"basic", new List<string>() {"LaunchGrende"} }
    };

    static public Dictionary<string, Action> _enemyPersonality = new Dictionary<string, Action>()
    {
        { "balanced", ()=>{} }
    };
}
