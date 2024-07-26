using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildList : MonoBehaviour
{
    List<Transform> _Tchildren;

    List<GameObject> _children;

    GameObject _deckContainer;

    GameObject _cardCollection;

    GameObject _collection;

    void Awake()
    {
        _deckContainer = GameObject.Find("DeckContainer");
        _Tchildren = new List<Transform>();
        _children = new List<GameObject>();
        GetRecursive_children(_deckContainer.transform);

        _cardCollection = GameObject.Find("CardsCollection");

        _collection = GameObject.Find("Collection");
        _collection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetRecursive_children(Transform parenttransform)
    {
        foreach (Transform child in parenttransform)
        {
            _Tchildren.Add(child.transform);
            if (child.transform.childCount > 0)
            {
                GetRecursive_children(child);
            }
        }
    }

    public void Click ()
    {
        _collection?.SetActive(true);
        foreach(Transform child in _Tchildren)
        {
            GameObject card = Instantiate(child.gameObject, _cardCollection.transform);
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero;
            _children.Add(card.transform.gameObject);
            card.SetActive(true);
        }
    }

    public void Exit()
    {
        _collection?.SetActive(false);
    }
}