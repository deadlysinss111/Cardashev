using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StatManager;

// If this stays THAT empty, move everything in Merchant.cs
// If some logic needs to be implemented, let it grow.
public class CanvasScript : MonoBehaviour
{
    /*
     FIELDS
    */
    [Header("Shopkeeper")]
    public GameObject _shopInterface;
    public TextMeshProUGUI _shopPlayerCredits;
    public GameObject _shopPlayerCreditIcon;

    Vector3 _firstIconPos = new Vector3(69, -62, 0);    // Shittest of all time, since it assumes the shape, size and pos of GO container "StatusEffects" in the Canvas (TL;DR: Don't touch "StatusEffetcs")
    GameObject[] _statusIconArr = new GameObject[Enum.GetNames(typeof(StatManager.Modifier.ModifierType)).Length];
    Dictionary<StatManager.Modifier.ModifierType, Color> _statusColorDict = new()
    {
        {StatManager.Modifier.ModifierType.Critical, Color.yellow },
        {StatManager.Modifier.ModifierType.RadPoison, Color.green },
    };

    /*
     METHODS
    */
    // Updates the HUD to match what's happening in-game
    private void Update()
    {
        // Updates the mirror, visual statusEffect array
        for (int i = 0;  i < _statusIconArr.Length; ++i)
        {
            if (GI._PStatFetcher()._statusEffectArr[i] != null)
            {
                // Check if the statusEffect is brand new and requires an Instantiation of its visual HUD partner
                if (_statusIconArr[i] == null)
                {
                    _statusIconArr[i] = (GameObject) Instantiate(Resources.Load("StatusIcon"), GameObject.Find("StatusEffects").transform);
                    _statusIconArr[i].GetComponent<RawImage>().color = _statusColorDict[GI._PStatFetcher()._statusEffectArr[i]._type];
                    _statusIconArr[i].transform.localPosition = _firstIconPos + new Vector3(i*30, 0, 0);
                }

                // Updates the text of the remaining time
                _statusIconArr[i].GetComponent<TMPro.TextMeshProUGUI>().text = GameTimer.GetFormattedTime(GI._PStatFetcher()._statusEffectArr[i]._durationLeft);
            }
            else
            {
                // Check if the statusEffect just died out and requires a Destruction of its visual HUD partner
                if (_statusIconArr[i] != null)
                    Destroy(_statusIconArr[i]);

                // Sets value to null just in case
                _statusIconArr[i] = null;
            }
        }
    }
}
