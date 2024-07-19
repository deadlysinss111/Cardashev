using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    protected List<GameObject> _hand;
    protected List<GameObject> _remainsInDeck;
    protected List<GameObject> _discardPile;

    protected float _drawCooldown;
    protected int _curHandSize;

    GameObject _cardBar;

    Vector3 virtualPosition = new Vector3(1000, -50, 0); // Virtual position off-screen to the right
    float transitionDuration = 0.3f; // Duration for the transition
    bool isTransitioning = false; // Flag to check if a card is currently being moved

    void Start()
    {
        _hand = new List<GameObject>();
        _discardPile = new List<GameObject>();
        _remainsInDeck = new List<GameObject>();
        _drawCooldown = 0;
        _curHandSize = 0;
        _cardBar = GameObject.Find("DrawCardBar");


        LoadDeck();

        DrawInit();
    }

    private void Update()
    {
        if(_hand.Count < 5)
            _drawCooldown -= Time.deltaTime;
        if (_drawCooldown <= 0)
        {
            if (_hand.Count < 5 && !isTransitioning)
            {
                Draw();
                _drawCooldown = 3;
            }
        }
        CooldownBarScale();
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _drawCooldown = 0;
            }
        }
    }

    public void Draw()
    {
        // If the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            if (_discardPile.Count == 0)
                return;
            _remainsInDeck = _discardPile;
            _discardPile = new List<GameObject> { };
        }

        // We draw a random card
        int rdm = UnityEngine.Random.Range(0, _remainsInDeck.Count);

        // We need to duplicate the card's game object so that we can display it and destroy it later easily
        GameObject obj = _remainsInDeck[rdm];
        obj.gameObject.SetActive(true);

        // Reset the scale of the card to the default value
        obj.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        _hand.Add(obj);

        // Since we drew it, we remove the card from the deck
        _remainsInDeck.RemoveAt(rdm);

        // Place the card at the virtual position
        obj.transform.localPosition = virtualPosition;

        // Start the coroutine to move the card to its final position
        StartCoroutine(MoveCardToHand(obj, _hand.Count - 1));
    }

    public void DrawInit()
    {
        for(byte i =0; i<4; i++)
        {
            // If the deck is empty, we shuffle the discard pile into it
            if (_remainsInDeck.Count == 0)
            {
                if (_discardPile.Count == 0)
                    return;
                _remainsInDeck = _discardPile;
                _discardPile = new List<GameObject> { };
            }

            // We draw a random card
            int rdm = UnityEngine.Random.Range(0, _remainsInDeck.Count);

            // We need to duplicate the card's game object so that we can display it and destroy it later easily
            GameObject obj = _remainsInDeck[rdm];
            obj.gameObject.SetActive(true);

            // Reset the scale of the card to the default value
            obj.transform.localScale = new Vector3(1.5f, 1.5f, 1);

            _hand.Add(obj);

            // Since we drew it, we remove the card from the deck
            _remainsInDeck.RemoveAt(rdm);

            // Place the card at the virtual position
            obj.transform.localPosition = new Vector3(-400 + 200 * i, -50, 0);
        }
    }

    IEnumerator MoveCardToHand(GameObject card, int index)
    {
        isTransitioning = true;
        Vector3 startPosition = card.transform.localPosition;
        Vector3 endPosition = new Vector3(-400 + 200 * index, -50, 0);
        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            card.transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.transform.localPosition = endPosition;
        isTransitioning = false;
    }

    void Discard(GameObject target)
    {
        _hand.Remove(target);
        _discardPile.Add(target);
        target.gameObject.SetActive(false);

        // Start the coroutine to move remaining cards to their new positions
        StartCoroutine(MoveHandCards());
    }

    IEnumerator MoveHandCards()
    {
        isTransitioning = true;
        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> endPositions = new List<Vector3>();

        // Store the start and end positions for each card
        for (int i = 0; i < _hand.Count; i++)
        {
            startPositions.Add(_hand[i].transform.localPosition);
            endPositions.Add(new Vector3(-400 + 200 * i, -50, 0));
        }

        float elapsedTime = 0;
        while (elapsedTime < transitionDuration)
        {
            for (int i = 0; i < _hand.Count; i++)
            {
                if (i >= startPositions.Count)
                    continue;
                _hand[i].transform.localPosition = Vector3.Lerp(startPositions[i], endPositions[i], (elapsedTime / transitionDuration));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final positions to ensure all cards are correctly placed
        for (int i = 0; i < _hand.Count; i++)
        {
            if (i >= startPositions.Count)
                continue;
            _hand[i].transform.localPosition = endPositions[i];
        }

        isTransitioning = false;
    }

    public void Play(Card target)
    {
        if (GI._PlayerFetcher().GetComponent<QueueComponent>().AddToQueue(target) == true)
        {
            Discard(target.gameObject);
        }
    }

    // Handling positions in order to have a good looking displaying
    /*private void DisplayHand()
    {
        for (byte i = 0; i < _hand.Count; i++)
        {
            if (!_hand[i].activeSelf)
            {
                continue;
            }
            _hand[i].transform.localPosition = new Vector3(-400 + 200 * i, -300, 0);
        }
    }*/

    public void LoadDeck()
    {
        List<GameObject> toLoad = GI._PManFetcher().GetDeck();
        foreach (GameObject card in toLoad)
        {
            _remainsInDeck.Add(card);
            card.transform.SetParent(GameObject.Find("PlayerHand").transform, false);
            card.GetComponent<Card>().OnLoad();
            if(card.TryGetComponent(out RotationSelectArrow arrow))
                arrow.InitArrow();
        }
    }

    public void UnloadDeck()
    {
        foreach (GameObject card in _remainsInDeck)
        {
            card.SetActive(false);
            card.transform.SetParent(GI._deckContainer.transform, false);
        }
        foreach (GameObject card in _hand)
        {
            card.SetActive(false);
            card.transform.SetParent(GI._deckContainer.transform, false);
        }
        foreach (GameObject card in _discardPile)
        {
            card.SetActive(false);
            card.transform.SetParent(GI._deckContainer.transform, false);
        }
    }

    // Method to display the draw cooldown in the UI with _cardBar
    private void CooldownBarScale()
    {
        if (_hand.Count < 5)
        {
            _cardBar.GetComponent<Slider>().value = _drawCooldown / 3;
        }
        else
        {
            _cardBar.GetComponent<Slider>().value = 1;
        }
    }
}
