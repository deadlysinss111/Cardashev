using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    // Logic Fields
    public static GameOverManager _instance;
    [NonSerialized] public bool _inGameOver;

    // UI & Canvas Fields
    private GameObject _gameOverPanel;
    private List<Button> _buttonList;
    private TMP_Text _text;


    void Start()
    {
        _instance = this;
        _inGameOver = false;

        _gameOverPanel = GameObject.Find("Panel");

        _buttonList = _gameOverPanel.GetComponentsInChildren<Button>().ToList();

        _text = GameObject.Find("TimeText").GetComponent<TMP_Text>();

        gameObject.SetActive(false);
    }

    public void StartGameOver()
    {
        gameObject.SetActive(true);
        _inGameOver = true;
        _text.text = "Time spend: "+GameObject.FindAnyObjectByType<GameTimer>().GetFormattedTime();
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(2.15f*2);
    }

    public void Restart()
    {
        _inGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        _inGameOver = true;
        SceneManager.LoadScene("Room & Tile tests");
    }
}
