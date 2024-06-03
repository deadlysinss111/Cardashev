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
        List<Card> pool;
        Collection._unlocked.TryGetValue(Idealist._name, out pool);

        for (int i = 0; i < 6; i++)
        {

            Card cardComp = pool[Random.Range(0, pool.Count)];
            UnityEngine.Object CARD = Resources.Load(cardComp._name);
            GameObject cardObj = (GameObject)Instantiate(CARD);
            cardObj.transform.SetParent(_interface.transform, false);

            int variation = UnityEngine.Random.Range(-10, 11);
            int amount = cardComp._goldValue + variation;

            cardComp.SetToCollectible(() =>
            {
                PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
                if (manager._goldAmount >= amount)
                {
                    manager._goldAmount -= amount;
                    Destroy(cardObj);
                    return true;
                }
                return false;
            });
        }
    }
}
