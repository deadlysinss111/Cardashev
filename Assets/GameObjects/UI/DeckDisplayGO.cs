using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplayGO : MonoBehaviour
{
    private GameObject _cardsContainer;

    public GameObject _cardRowPrefab;
    public GameObject _cardPrefab;
    public CardManager _cardManager;

    GameObject _deckContainer;

    private void Awake()
    {
        _cardsContainer = GameObject.Find("CardsCollection");

        _deckContainer = GI._deckContainer;

    }

    private void AddCardToContainer(GameObject card)
    {
        byte carPerRow = 6;


    }
}
