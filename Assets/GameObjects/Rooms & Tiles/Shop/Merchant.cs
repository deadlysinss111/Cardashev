using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Merchant : Interactible
{
    GameObject _interface;

    private new void Awake()
    {
        base.Awake();
        _interface = GameObject.Find("ShopInterface");
        _interface.SetActive(false);
    }

    public override void OnRaycastHit()
    {
        DrawOfferedCards();
    }

    void DrawOfferedCards()
    {
        _interface.SetActive(true);
        List<string> pool;
        Collection._unlocked.TryGetValue(Idealist._name, out pool);
        int x = -300;

        for (int i = 0; i < 6; i++)
        {
            // We first create the card object
            UnityEngine.Object CARD = Resources.Load(pool[Random.Range(0, pool.Count)]+"Model");
            GameObject cardObj = (GameObject)Instantiate(CARD);
            cardObj.transform.SetParent(_interface.transform, false);
            cardObj.transform.localPosition = new Vector3(x, 100, 0);
            cardObj.transform.localScale = new Vector3(5, 1, 5);

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
            priceObj.transform.localPosition = new Vector3(x-5, 60, 0);

            // We set the card as a collectible with a price
            cardComp.SetToCollectible(() =>
            {
                PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
                if (manager._goldAmount >= amount)
                {
                    manager._goldAmount -= amount;
                    Destroy(cardObj);
                    Destroy(priceObj);
                    print(manager._goldAmount);
                    return true;
                }
                return false;
            });

            x += 100;
        }
    }
}
