using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// If this stays THAT empty, move everything in Merchant.cs
// If some logic needs to be implemented, let it grow.
public class CanvasScript : MonoBehaviour
{
    /*
     FIELDS
    */
    // Fields for the shop interface
    [Header("Shopkeeper")]
    public GameObject _shopInterface;
    public TextMeshProUGUI _shopPlayerCredits;
    public GameObject _shopPlayerCreditIcon;

    // Fields for displaying status effect to the player
    List<GameObject> _visualStatusEffect;
    bool _visualStatusEffectChanged;
    Dictionary<StatManager.Modifier.ModifierType, Color> _statusEffectColor = new() {
        { StatManager.Modifier.ModifierType.Critical, Color.yellow }
    };
    Vector3 defaultStatusIconPos = new Vector3(99.0f, -55.0f, 0.0f);

    /*
     METHODS
    */
    // Calls for updating positions, and sometimes destroys the indicator
    private void Update()
    {
        if (_visualStatusEffectChanged)
        {
            foreach(Transform visualStatus in _visualStatusEffect)
            {
                if (GI._PStatFetcher().IsUnderStatusEffect()
            }
        }
    }

    // "Constructor" of the visual elements. Always called right as we apply a status effect to the player
    public void CreateVisualStatusEffect(StatManager.Modifier.ModifierType ARGstatusEffect)
    {
        // Creating a GO that shows effect icon and also will be parent to the timer and the periodical indicator (if needed)
        GameObject newStatusIcon = new();
        RawImage compRawImage = newStatusIcon.AddComponent<RawImage>();
        compRawImage.color = _statusEffectColor[ARGstatusEffect];

        // Creating a GO that will contain the game timer
        GameObject newStatusTimer = new();
        TextMeshPro compTextMeshPro = newStatusTimer.AddComponent<TextMeshPro>();
        compTextMeshPro.text = "00:00";
        compTextMeshPro.fontSize = 8;
        compTextMeshPro.alignment = TextAlignmentOptions.Center;

        // Adding the element to the list, moving it correctly, and instiating it now that it's all ready :3
        _visualStatusEffect.Add(newStatusTimer);
        GameObject actualStatusIcon = Instantiate(newStatusIcon, this.transform.Find("PlayerStatus"));
        actualStatusIcon.name = ARGstatusEffect.ToString();

        GameObject actualStatusTimer = Instantiate(newStatusTimer, actualStatusIcon .transform);
        actualStatusTimer.name = "Timer";
    }
}
