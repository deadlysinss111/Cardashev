using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Merchant : Interactible
{
    GameObject _interface;
    CanvasScript _canvasData;
    GameObject[] _shopSupply;

    private void Start()
    {
        _interface = GameObject.Find("ShopInterface");
        _canvasData = GameObject.Find("Canvas").GetComponent<CanvasScript>();
        _RaycastHitDist = 10;
        _inRangeCursor = "Dialog";
        _shopSupply = new GameObject[4];
        Collection._unlocked.TryGetValue(Idealist._instance._name, out List<string> pool);
        for (int i=0; i<4; i++)
        {
            _shopSupply[i] = Card.Instantiate(pool[Random.Range(0, pool.Count)], true);
            _shopSupply[i].transform.SetParent(_interface.transform, false);
            _shopSupply[i].transform.localPosition = new Vector3(-300 + i*200, 100, 0.1f);
            _shopSupply[i].GetComponentInChildren<CanvasGroup>().alpha = 1;
            
            Card cardComp = _shopSupply[i].GetComponent<Card>();

            _shopSupply[i].GetComponent<CardData>()._priceText.text = cardComp._goldValue.ToString();
            GameObject curCard = _shopSupply[i];
            cardComp.SetToCollectible(() =>
            {
                if (CurrentRunInformations._goldAmount >= cardComp._goldValue)
                {
                    CurrentRunInformations._goldAmount -= cardComp._goldValue;
                    //Destroy(cardObj);
                    _canvasData._shopPlayerCredits.text = CurrentRunInformations._goldAmount.ToString();
                    cardComp.gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0;
                    return Card.CollectibleState.ADDTODECKANDBACKTOPLAY;
                }
                curCard.GetComponent<Animator>().SetTrigger("Shake");
                return Card.CollectibleState.NOTHING;
            });
        }
    }

    public override void OnRaycastHit()
    {
        if (Vector3.Distance(this.transform.position, _playerRef.transform.position) <= _RaycastHitDist)
            StartCoroutine(BandAidOffsetDrawing());
        else
            print("you're too far from the merchant");
    }

    // Band aid that prevent the click effect to be called right awa (hate unity btw)
    IEnumerator BandAidOffsetDrawing()
    {
        byte offset = 2;
        while (offset-- >0)
        {
            yield return null;
        }
        DrawOfferedCards();
    }

    void DrawOfferedCards()
    {
        GI._PManFetcher().SetToState("Empty");
        CanvasGroup shopInterface = _interface.GetComponent<CanvasGroup>();
        shopInterface.alpha = 1;
        shopInterface.blocksRaycasts = true;
        shopInterface.interactable = true;
        _canvasData._shopPlayerCreditIcon.SetActive(true);

        // Show to the player his current money
        _canvasData._shopPlayerCredits.text = CurrentRunInformations._goldAmount.ToString();
    }
}
