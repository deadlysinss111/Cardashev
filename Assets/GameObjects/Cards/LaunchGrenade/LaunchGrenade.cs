using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchGrenade : Card
{
    //GameObject _previwRadius;

    GameObject _grenadePrefab;
    [SerializeField] GameObject _explosion;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"damage", 15},
            {"innerRange", 8},
            {"outerRange", 15},
            {"explosionRadius", 14},
        };

        string desc = $"Launch an impact grenade dealing {stats["damage"]}";
        base.Init(1, 2, 60, stats, desc, PreviewZoneType.SPHERE);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("grenade" + _id.ToString(), EnterGrenadeState, ExitState) == false) _id++;

        _grenadePrefab = (GameObject)Resources.Load("Grenade");

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    void EnterGrenadeState()
    {
        PlayerManager manager = GI._PManFetcher();
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, _stats["innerRange"], _stats["outerRange"]);

        manager.SetLeftClickTo(FireGrenade);
        manager.SetRightClickTo(() => { 
            ExitState();
            GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
            if (_ghostHitbox != null)
                Destroy(_ghostHitbox);
        });
        manager.SetHoverTo(Preview);
        //_previwRadius.SetActive(true);
        GI.UpdateCursors("Bomb", (byte)(GI.CursorRestriction.S_TILES));
        GI.UpdateCursorsInverted("Cross", (byte)(GI.CursorRestriction.S_TILES));

        //_ghostHitbox = (GameObject)Instantiate(_ghostHitboxPrefab);
    }

    new protected void Preview()
    {
        base.Preview();

        _ghostHitbox.transform.localScale = new Vector3(_stats["explosionRadius"], _stats["explosionRadius"], _stats["explosionRadius"]);
    }

    void ExitState()
    {
        //_previwRadius.SetActive(false);
        _selectableArea.ResetSelectable();
    }

    public override void Effect()
    {
        GameObject grenade = Instantiate(_grenadePrefab);
        grenade.GetComponent<Rigidbody>().transform.position = _originFromLastBellCurveCalculated + new Vector3(0, 5, 0);
        GrenadeScript grenadeScript = grenade.GetComponent<GrenadeScript>();
        grenadeScript._velocity = _velocityFromLastBellCurveCalculated;
        grenadeScript._dmg = _stats["damage"];
        grenadeScript._origin = _originFromLastBellCurveCalculated + new Vector3(0, 1, 0);
        grenadeScript._explosionRadius = _stats["explosionRadius"];
        grenadeScript._explosionEffect = _explosion;

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("grenade" + _id.ToString());
    }
    

    protected void FireGrenade()
    {
        if (_selectableArea.CheckForSelectableTile(_destinationFromLastBellCurveCalculated) == false) return;
        _selectableArea.ResetSelectable();
        GI._PManFetcher().SetToDefault();
        // Trigger the card play event
        base.PlayCard();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    public override void OnLoad()
    {

    }

}