using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stats : MonoBehaviour
{
    UnityEvent Test;

    // Start is called before the first frame update
    void Start()
    {
        Test = new UnityEvent();
        DontDestroyOnLoad(gameObject);
        Test.AddListener(Ping);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Test.Invoke();
        }
    }

    void Ping()
    {
        print("Pong");
    }
}
