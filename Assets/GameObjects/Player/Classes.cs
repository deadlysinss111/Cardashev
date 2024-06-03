using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


static public class Idealist
{
    static public string _name = "brawler";
    static public byte _HP;
    static public Action _ultimate;
}

public struct UltiContext
{
    Vector3 _targetPos;

    public UltiContext(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}


static public class ClassFactory
{
    static public void Brawler()
    {
        Idealist._name = "brawler";
        Idealist._HP = 100;
        Idealist._ultimate = () => { Debug.Log("ulti used"); };
    }

    public static UltiContext _context;

    static public Dictionary<string, Action> _classesBook = new Dictionary<string, Action>()
    {
        { "brawler", Brawler }
    };
} 
