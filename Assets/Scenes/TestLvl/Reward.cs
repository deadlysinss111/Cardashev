using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class Reward : MonoBehaviour
{
    public struct Content
    {
        internal int[] _goldRange;
        internal byte _rewardLvl;

        public Content(int low, int high, byte rewardLvl)
        {
            _goldRange = new int[2];
            _goldRange[0] = low;
            _goldRange[1] = high;
            _rewardLvl = rewardLvl;

        }
    }


    List<GameObject> _buttons;
    GameObject _leaveButton;
    bool _left = false;

    [SerializeField] GameObject _winScreen;
    [SerializeField] GameObject _cardSelectionPanel;

    static public Content _content = new Content(0, 0, 0);

    // Loaders
    GameObject BUTTON;
    GameObject CARD;


    private void Awake()
    {
        _buttons = new List<GameObject>();
    }

    private void Start()
    {
        BUTTON = (GameObject)Resources.Load("ButtonPrefab");
        CARD = (GameObject)Resources.Load("LaunchGrenadeCard");
        StartCoroutine(DisplayScreen());
    }

    public void TriggerRewards()
    {
        _winScreen.SetActive(true);
        StartCoroutine(DisplayScreen());
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
        _leaveButton.transform.localPosition = new Vector3(0, -280, 0);
        _leaveButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Leave");
        _leaveButton.GetComponent<Button>().onClick.AddListener( () =>{ GI._loader.LoadScene("Reward", "Map", LoadSceneMode.Single); });
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
        GameObject button = Instantiate(BUTTON);
        button.transform.SetParent(GameObject.Find("Button Area").transform, false);
        if (addItToButtonList)
        {
            _buttons.Add(button);
        }
        return button;
    }

    void GenerateGolds()
    {
        GameObject button = GenerateItem(true);
        int amount = UnityEngine.Random.Range(_content._goldRange[0], _content._goldRange[1]);
        button.GetComponent<Button>().onClick.AddListener(() => { CurrentRunInformations._goldAmount += amount; Destroy(button); _buttons.Remove(button); DisplayButtons(); });
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText($"{amount} Gold");
    }

    void GenerateBooster()
    {
        GameObject button = GenerateItem(true);
        button.GetComponent<Button>().onClick.AddListener(() => { DisplayCards(button); _cardSelectionPanel.GetComponent<CanvasGroup>().alpha = 0.4f; Destroy(button); });
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Pick a card");
    }

    void DisplayButtons()
    {
        if(_buttons.Count > 0)
        {
            int y = 200;
            foreach (GameObject button in _buttons)
            {
                button.transform.localPosition = new Vector3(0, y, 0);
                y -= 80;
            }
        }
    }

    void DisplayCards(GameObject button)
    {
        GameObject[] cards = new GameObject[3];

        GameObject upgrade = GenerateItem(false);
        upgrade.GetComponent<Button>().onClick.AddListener(() => { foreach (GameObject slot in cards) { Destroy(slot); }; DisplayUpgradableCards(); Destroy(upgrade); });
        upgrade.transform.localPosition = new Vector3(0, -170, -0.1f);
        upgrade.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Upgrade");

        Collection._unlocked.TryGetValue(Idealist._instance._name, out List<string> pool);

        for (int i = 0; i < 3; i++)
        {
            GameObject card = Card.Instantiate(pool[UnityEngine.Random.Range(0, pool.Count)], true);
            card.layer = LayerMask.NameToLayer("UI");
            card.transform.SetParent(GameObject.Find("Canvas").transform, false);
            card.transform.localPosition = new Vector3(200 * (i - 1), -50, -0.1f);
            card.GetComponent<Card>().SetToCollectible(() => { 
                foreach (GameObject slot in cards) if(slot != card){ Destroy(slot); };
                _cardSelectionPanel.GetComponent<CanvasGroup>().alpha = 0f;
                Destroy(button);
                Destroy(upgrade);
                card.SetActive(false);
                return Card.CollectibleState.ADDTODECKANDBACKTOPLAY; 
            });
            cards[i] = card;

            // MERGE ARTIFACT
            // GameObject card = Instantiate(CARD);
            // card.layer = LayerMask.NameToLayer("UI");
            // card.transform.SetParent(GameObject.Find("Card Selection Panel").transform, false);
            // card.transform.localPosition = new Vector3(150*(i-1), 0, -0.1f);
            // card.GetComponent<Card>().SetToCollectible(() => { 
            //     foreach (GameObject slot in cards) 
            //     { 
            //         Destroy(slot);
            //     }
            //     Destroy(button);
            //     _cardSelectionPanel.GetComponent<CanvasGroup>().alpha = 0f;
            //     return true; 
            // });
        }
        
    }

    void DisplayUpgradableCards()
    {
        List<GameObject> deckPool = CurrentRunInformations._deck;
        GameObject[] cards = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            int rdm = UnityEngine.Random.Range(0, deckPool.Count);
            GameObject card = deckPool[rdm];
            deckPool.RemoveAt(rdm);

            card.SetActive(true);
            card.transform.SetParent(GameObject.Find("Canvas").transform, false);
            card.transform.localPosition = new Vector3(200 * (i - 1), -50, -0.1f);

            card.GetComponent<Card>().SetToCollectible(() =>
            {
                card.GetComponent<Card>().Upgrade();
                foreach (GameObject slot in cards)
                {
                    slot.transform.SetParent(GI._deckContainer.transform, false);
                    print("NOT ALLOWED");
                    slot.SetActive(false);
                };
                _cardSelectionPanel.SetActive(false);
                return Card.CollectibleState.ADDTODECKANDBACKTOPLAY;
            });

            cards[i] = card;
        }
    }
}
