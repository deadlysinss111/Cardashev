using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Windows.Speech;

// replace all TEMPLATE

public class SecondSleeve : Card
{
    Vector3 _direction;
    Vector3 _startingPosition;
    int _damage = 10;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        //int[] stats = new int[2];
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"damage", 32}
        };
        /* stats fill there */
        base.Init(CardType.OFFENSE, 1, 2, 60, stats, "");


        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("SecondSleeve" + _id.ToString(), EnterState, ExitState) == false) _id++;

        if (gameObject.TryGetComponent(out _rotationArrow) == false)
            _rotationArrow = gameObject.AddComponent<RotationSelectArrow>();
    }

    void EnterState()
    {
        _rotationArrow.SetArrow(true);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(LeftClick);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(DisplayRange);
        GI.UpdateCursorsInverted("Bow", (byte)(GI.CursorRestriction.S_TILES));
    }

    void LeftClick()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetToDefault();

        if (manager._lastHit.transform == null) return;

        _direction = Vector3.Normalize(manager._lastHit.point - manager._virtualPos);
        _direction.y = 0;
        _startingPosition = manager._virtualPos;

        base.PlayCard();
    }

    void ExitState()
    {
        _rotationArrow.SetArrow(false);
        ClearPath();
    }

    void DisplayRange()
    {

    }

    public override void Effect()
    {
        GameObject bullet = Instantiate((GameObject)Resources.Load("Bullet"));

        bullet.GetComponent<Bullet>().SetDirection(_direction);
        _startingPosition.y += 1.5f;
        bullet.GetComponent<Bullet>().SetInitialValues(_startingPosition, 10, _damage);

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("SecondSleeve" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}