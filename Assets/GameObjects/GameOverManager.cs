using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    [NonSerialized] public bool inGameOver;
    [NonSerialized] public GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        inGameOver = false;

        gameOverPanel = GameObject.Find("Panel");
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameOver()
    {
        gameObject.SetActive(true);
        inGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        print("wait");
        yield return new WaitForSeconds(2.15f*2);
        print("no more wait");
    }
}
