using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CardLoader
{
    static public GameObject Load(Card card)
    {
        GameObject container = new GameObject(card.name + " Container");

        MonoBehaviour.Instantiate((GameObject)Resources.Load("spriteimg"), container.transform);

        switch (card._currLv)
        {
            case 4:
                MonoBehaviour.Instantiate((GameObject)Resources.Load("lvl4"), container.transform);
                break;
            case 3:
                MonoBehaviour.Instantiate((GameObject)Resources.Load("lvl3"), container.transform);
                break;
            case 2:
                MonoBehaviour.Instantiate((GameObject)Resources.Load("lvl2"), container.transform);
                break;
        }

        

        return container;
    }
}
