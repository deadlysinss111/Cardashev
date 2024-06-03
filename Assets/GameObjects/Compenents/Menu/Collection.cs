using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    Canvas _canvas;

    List<GameObject> _currentUnlocked;
    List<GameObject> _currentLocked;

    public static Dictionary<string, List<Card>> _unlocked;
    public static Dictionary<string, List<Card>> _locked;

    void Load(string toLoad)
    {
        _unlocked = new Dictionary<string, List<Card>>();
        _locked = new Dictionary<string, List<Card>>();

        List<Card> cards;

        _unlocked.TryGetValue(toLoad, out cards);
        foreach(Card target in cards)
        {
            UnityEngine.Object CARD = Resources.Load(target.name);
            GameObject card = (GameObject)Instantiate(CARD);
            card.transform.SetParent(_canvas.transform, false);
            _currentUnlocked.Add(card);
        }

        _locked.TryGetValue(toLoad, out cards);
        foreach(Card target in cards)
        {
            UnityEngine.Object CARD = Resources.Load(target.name);
            GameObject card = (GameObject)Instantiate(CARD);
            card.transform.SetParent(_canvas.transform, false);
            // Either do something here to distinguish locked card
            _currentLocked.Add(card);
        }
    }
    
    // All below values for positions are totally random and meant to be changed later on
    void Display()
    {
        int x = 0;
        int y = 0;
        foreach(GameObject target in _currentUnlocked)
        {
            target.transform.localPosition = new Vector3(x, y, 0);
            x += 150;
            if(x >= 1350)
            {
                x = 0;
                y += 200;
            }
        }
        foreach(GameObject target in _currentLocked)
        {
            target.transform.localPosition = new Vector3(x, y, 0);
            // Either do something here to distinguish locked card
            x += 150;
            if (x >= 1350)
            {
                x = 0;
                y += 200;
            }
        }
    }
}
