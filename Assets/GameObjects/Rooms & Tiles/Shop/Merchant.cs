using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Merchant : Interactible
{
    GameObject _interface;

    private void Start()
    {
        _interface = GameObject.Find("Canvas").GetComponent<CanvasScript>()._shopInterface;
        _RaycastHitDist = 4;
        _inRangeCursor = "Dialog";
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
        _interface.SetActive(true);
        Collection._unlocked.TryGetValue(Idealist._instance._name, out List<string> pool);
        int x = -300;

        // Show to the player his current money
        GameObject playerGoldbagObj = new GameObject();
        TextMesh playerGoldbagTxt = playerGoldbagObj.AddComponent<TextMesh>();
        playerGoldbagObj.transform.SetParent(_interface.transform, false);
        playerGoldbagTxt.text = CurrentRunInformations._goldAmount.ToString();
        playerGoldbagTxt.color = Color.red;
        playerGoldbagTxt.fontSize = 300;
        playerGoldbagObj.transform.localPosition = new Vector3(-500, 200, 0.1f);

        for (int i = 0; i < 4; i++)
        {
            // We first create the card object
            GameObject cardObj = Card.Instantiate(pool[Random.Range(0, pool.Count)], true);
            cardObj.transform.SetParent(_interface.transform, false);
            cardObj.transform.localPosition = new Vector3(x, 100, 0.1f);

            // We set it's gold value
            Card cardComp = cardObj.GetComponent<Card>();
            int variation = UnityEngine.Random.Range(-10, 11);
            int amount = cardComp._goldValue + variation;

            // We then write the price
            GameObject priceObj = new GameObject();
            priceObj.transform.SetParent(_interface.transform, false);
            TextMesh priceTxt = priceObj.AddComponent<TextMesh>();
            priceTxt.text = amount.ToString();
            priceTxt.color = Color.red;
            priceTxt.fontSize = 200;
            priceObj.transform.localPosition = new Vector3(x - 5, 60, 0.1f);

            // We set the card as a collectible with a price
            cardComp.SetToCollectible(() =>
            {
                if (CurrentRunInformations._goldAmount >= amount)
                {
                    CurrentRunInformations._goldAmount -= amount;
                    //Destroy(cardObj);
                    Destroy(priceObj);
                    playerGoldbagTxt.text = CurrentRunInformations._goldAmount.ToString();
                    return Card.CollectibleState.ADDTODECKANDBACKTOPLAY;
                }
                cardObj.GetComponent<Animator>().SetTrigger("Shake");
                return Card.CollectibleState.NOTHING;
            });

            x += 200;
        }
    }
}
