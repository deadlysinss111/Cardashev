using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-999)]
public class GlobalStats : MonoBehaviour
{
    /*
     FIELDS
    */
    // Constructor to inititalize the Dicts (in fine, using a save file)
    // TODO
    readonly static Dictionary<string, UnityEvent> _eventCallers = new();
    readonly static Dictionary<string, int> _statsInt = new(); // Envisager un Dict<string, Union> ?


    /*
     METHODS
    */
    void Start()
    {
        //DontDestroyOnLoad(gameObject);

        CreateStat("mouvements", 0);
        CreateStat("jumps", 0);
    }

    /// <summary>
    /// Creates a stat to follow with an event
    /// </summary>
    /// <param name="name">The id of the stat</param>
    /// <param name="value">Its default value</param>
    void CreateStat(string name, int value)
    {
        _statsInt.Add(name, value);
        _eventCallers.Add(name, new UnityEvent());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Supposed to call a Ue");
        }
    }

    /// <summary>
    /// Changes a stat and invoke all functions subscribed to its event
    /// </summary>
    /// <param name="id">The id of the stat to modify</param>
    /// <param name="newValue">Rge new alue of the stat</param>
    /// <param name="add">If true, newValue will be added to the current value. If false, it will replace it.</param>
    /// <exception cref="KeyNotFoundException"></exception>
    public static void UpdateStat(string id, int newValue, bool add=true)
    {
        if (_statsInt.ContainsKey(id) == false)
        {
            throw new KeyNotFoundException($"The stat {id} does not exist.");
        }
        if (add)
        {
            _statsInt[id] += newValue;
        }
        else
        {
            _statsInt[id] = newValue;
        }
        _eventCallers[id].Invoke();
    }

    public static int GetStat(string name)
    {
        return _statsInt[name];
    }

    public static void ListenToStat(string stat, UnityAction callback)
    {
        _eventCallers[stat].AddListener(callback);
    }
}
