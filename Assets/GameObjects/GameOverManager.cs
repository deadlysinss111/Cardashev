using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    GameObject _hud;

    PlayerManager _playerManager;

    DeckManager _deckManager;

    GameObject _menu;
    GameObject _collection;
    GameObject _cardsCollection;

    void Start()
    {
        _instance = this;
        _inGameOver = false;

        _gameOverPanel = GameObject.Find("Panel");

        _buttonList = _gameOverPanel.GetComponentsInChildren<Button>().ToList();

        //_text = GameObject.Find("GameTimer").GetComponent<TMP_Text>();

        _hud = GameObject.Find("HUD");

        _menu = GameObject.Find("Menu");

        _collection = GameObject.Find("Collection");

        _cardsCollection = GameObject.Find("CardsCollection");

        _collection.SetActive(false);

        GameObject player = GameObject.Find("Player");
        _playerManager = player.GetComponent<PlayerManager>();
        _deckManager = player.GetComponent<DeckManager>();

        CanvasGroup group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

    }

    public void StartGameOver()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        _inGameOver = true;
        //_text.text = "Time spend: "+GameObject.Find("GlobalTimer").GetComponent<GameTimer>().GetFormattedTime();
        StopGame();
        _deckManager.UnloadDeck();
        DisableAllScripts();

    }

    public void Restart()
    {
        _inGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        _inGameOver = true;
        SceneManager.LoadScene("EntryPoint");
    }

    void StopGame()
    {
        Time.timeScale = 0;
        _hud.SetActive(false);
    }

    void DisableAllScripts()
    {
        _playerManager.SetToState("Empty");
    }

    public void DeckDisplay()
    {
        _menu.SetActive(false);
        GI._deckContainer.SetActive(true);
        GI._deckContainer.transform.SetParent(_cardsCollection.transform);
        _collection.SetActive(true);
    }

    public void ExitButton()
    {
        _menu.SetActive(true);
        _collection.SetActive(false);
    }
}
