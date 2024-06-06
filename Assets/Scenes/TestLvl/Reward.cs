using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    PlayerManager _manager;
    List<GameObject> _buttons;
    GameObject _leaveButton;
    bool _left = false;

    [SerializeField] GameObject _winScreen;
    [SerializeField] GameObject _cardBG;


    private void Awake()
    {
        _buttons = new List<GameObject>();
        _manager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            _winScreen.SetActive(true);
            StartCoroutine(DisplayScreen());
        }
    }

    // Displays the rewards screen
    IEnumerator DisplayScreen()
    {
        while(_winScreen.transform.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length > 
            _winScreen.transform.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime + 1.7)
        {
            yield return null;
        }

        GenerateRewards();
        _leaveButton = GenerateItem(false);
        _leaveButton.transform.localPosition = new Vector3(0, -300, 0);
        _leaveButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Leave");
        _leaveButton.GetComponent<Button>().onClick.AddListener( () =>
            { Loader loaderScript = GameObject.Find("Loader").GetComponent<Loader>();
              loaderScript._UeSceneChange.Invoke("Room", "Map", LoadSceneMode.Single); });
    }

    void GenerateRewards()
    {
        if(!_left)
        {
            GenerateGolds();
            GenerateBooster();
            DisplayButtons();
            _left = true;
        }
    }

    GameObject GenerateItem(bool addItToButtonList)
    {
        UnityEngine.Object BUTTON = Resources.Load("ButtonPrefab");
        GameObject button = (GameObject)Instantiate(BUTTON);
        button.GetComponent<Button>().onClick.AddListener(() => { Destroy(button); _buttons.Remove(button); DisplayButtons(); });
        button.transform.SetParent(GameObject.Find("Canvas").transform, false);
        if (addItToButtonList)
        {
            _buttons.Add(button);
        }
        return button;
    }

    void GenerateGolds()
    {
        GameObject button = GenerateItem(true);
        int amount = UnityEngine.Random.Range(10, 20);
        button.GetComponent<Button>().onClick.AddListener(() => { _manager._goldAmount = amount; });
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(amount.ToString());
    }

    void GenerateBooster()
    {
        GameObject button = GenerateItem(true);
        int amount = UnityEngine.Random.Range(10, 20);
        button.GetComponent<Button>().onClick.AddListener(() => { DisplayCards(); });
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("pick a card");
    }

    void DisplayButtons()
    {
        if(_buttons.Count > 0)
        {
            int y = 300;
            foreach (GameObject button in _buttons)
            {
                button.transform.localPosition = new Vector3(0, y, 0);
                y -= 100;
            }
        }
    }

    void DisplayCards()
    {
        _cardBG.SetActive(true);
        GameObject[] cards = new GameObject[3];
        for(int i = 0; i < 3; i++) 
        {
            GameObject CARD = (GameObject)Resources.Load("LaunchGrenadeModel");
            GameObject card = Instantiate(CARD);
            card.layer = LayerMask.NameToLayer("UI");
            card.transform.SetParent(GameObject.Find("Canvas").transform, false);
            card.transform.localScale = new Vector3(10, 1, 10);
            card.transform.localPosition = new Vector3(150*(i-1), 0, -0.1f);
            card.GetComponent<Card>().SetToCollectible(() => { foreach (GameObject slot in cards) { Destroy(slot); }; _cardBG.SetActive(false); return true; });
            cards[i] = card;
        }
    }
}
