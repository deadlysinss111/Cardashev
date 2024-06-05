using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalStats : MonoBehaviour
{
    UnityEvent Test;

    private static Dictionary<string, UnityEvent> _eventCallers = new();

    private static Dictionary<string, int> _statsInt = new();

    // Start is called before the first frame update
    void Start()
    {
        Test = new UnityEvent();
        DontDestroyOnLoad(gameObject);
        Test.AddListener(Ping);

        CreateStat("mouvements", 0);
        CreateStat("jumps", 0);
    }

    void CreateStat(string name, int value)
    {
        _statsInt.Add(name, value);
        _eventCallers.Add(name, new UnityEvent());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Test.Invoke();
        }
    }

    public static void UpdateStat(string id, int newValue, bool add=false)
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

    void Ping()
    {
        print("Pong");
    }

    public static int GetStat(string name)
    {
        return _statsInt[name];
    }
}
