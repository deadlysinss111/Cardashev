using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


// Will be Inherited by every specific .cs descirbing actual characters
// This is pretty empty since they need to share few things
public abstract class Idealist
{
    // Selected Idealist
    static public Idealist _instance;

    public string _name;
    public byte _baseHP;

    public abstract void Ultimate();

    static public void StartWith(string name)
    {
        Action func;
        _idealistEncyclopedia.TryGetValue(name, out func);
        func();
    }

    static private Dictionary<string, Action> _idealistEncyclopedia = new Dictionary<string, Action>()
    {
        { "Dix", ()=>{ _instance = new Dix(); } },
    };
}