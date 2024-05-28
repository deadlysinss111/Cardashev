using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Class
{
    public string _name;
    public byte _HP;
    public Func<UltiContext, bool> _ultimate;

    internal Class(string name, byte hp, Func<UltiContext, bool> ulti)
    {
        _name = name;
        _HP = hp;
        _ultimate = ulti;
    }
}

public struct UltiContext
{
    Vector3 _targetPos;

    public UltiContext(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}

public static class ClassFactory
{
    public static UltiContext _context;
    public static Class Brawler()
    {
        return new Class("brawler", 100, (UltiContext ctx) => {

            UnityEngine.Object grenadePrefab = Resources.Load("Grenade");
            GameObject grenade = (GameObject)UnityEngine.Object.Instantiate(grenadePrefab);

            return true;
        });
    }
} 
