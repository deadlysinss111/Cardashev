using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


// Will be Inherited by every specific .cs describing actual characters
// This is pretty empty since they need to share few things
public abstract class Idealist
{
    // Selected Idealist
    static public Idealist _instance;

    public string _name;
    public int _baseHP;

    public List<string> _startingDeck;
    public List<string> _collection;

    public abstract void Ultimate();

    static private Dictionary<string, List<string>> _idealistCollectionFetcher = new Dictionary<string, List<string>>()
    {
        { "Dix", new Dix()._collection},
    };

    static private Dictionary<string, Action> _idealistEncyclopedia = new Dictionary<string, Action>()
    {
        { "Dix", ()=>{ _instance = new Dix(); } },
    };

    static public void StartWith(string name)
    {
        Action func;
        _idealistEncyclopedia.TryGetValue(name, out func);
        func();

        CurrentRunInformations.AddCardsToDeck(_instance._startingDeck);
        CurrentRunInformations._playerHP = _instance._baseHP;
    }

    static public void CollectionDeck(string name)
    {
        if (_idealistCollectionFetcher.TryGetValue(name, out List<string> collec))
        {
            //bool ratio = true;
            CardCollection.AddCardsToCollection(collec);
        }
        else
            throw new Exception("you've got an error in the naming of your idealist: " + name);

    }
}