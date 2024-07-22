using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleShot : Card
{
    Vector3 _direction;
    Vector3 _startingPosition;
    int _damage = 10;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        int[] stats = new int[0];
        /* stats fill there */
        base.Init(1, 2, 60, stats);


        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("SimpleShot" + _id.ToString(), EnterState, ExitState) == false) _id++;

        //if (gameObject.TryGetComponent(out _rotationArrow) == false)
        //    _rotationArrow = gameObject.AddComponent<RotationSelectArrow>();
    }

    void EnterState()
    {
        //_rotationArrow.SetArrow(true);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(LeftClick);
        manager.SetRightClickTo(() => { 
            ExitState(); 
            GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
            if (_ghostHitbox != null)
                Destroy(_ghostHitbox);
        });
        manager.SetWallsAsClickable(true);
        manager.SetHoverTo(Preview);
        GI.UpdateCursorsInverted("Bow", (byte)(GI.CursorRestriction.S_TILES));
    }

    void LeftClick()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetToDefault();

        if (manager._lastHit.transform == null) return;

        _direction = Vector3.Normalize(manager._lastHit.point - manager._virtualPos);
        //_direction.y = 0;
        _startingPosition = manager._virtualPos+ new Vector3(0, 1, 0);

        base.PlayCard();
    }

    void ExitState()
    {
        //_rotationArrow.SetArrow(false);
        GI._PManFetcher().SetWallsAsClickable(false);
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
        GI._PManFetcher().SetToState("SimpleShot" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }
}