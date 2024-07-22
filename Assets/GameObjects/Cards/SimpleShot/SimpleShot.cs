using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleShot : Card
{
    Vector3 _direction;
    Vector3 _startingPosition;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"damage", 10}
        };
        /* stats fill there */
        base.Init("SimpleShot", 1, 2, 60, stats);

        //if (gameObject.TryGetComponent(out _rotationArrow) == false)
        //    _rotationArrow = gameObject.AddComponent<RotationSelectArrow>();
    }

    override protected void EnterState()
    {
        //_rotationArrow.SetArrow(true);
        GI._PManFetcher().SetWallsAsClickable(true);

        base.EnterState();
    }

    override protected void OnLeftClick()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetToDefault();

        if (manager._lastHit.transform == null) return;

        _direction = Vector3.Normalize(manager._lastHit.transform.position - manager._virtualPos);
        //_direction.y = 0;
        _startingPosition = manager._virtualPos+ new Vector3(0, 1, 0);

        base.PlayCard();
    }

    override protected void ExitState()
    {
        GI._PManFetcher().SetWallsAsClickable(false);

        base.ExitState();
    }

    void DisplayRange()
    {

    }

    public override void Effect()
    {
        GameObject bullet = Instantiate((GameObject)Resources.Load("Bullet"));

        bullet.GetComponent<Bullet>().SetDirection(_direction);
        _startingPosition.y += 1.5f;
        bullet.GetComponent<Bullet>().SetInitialValues(_startingPosition, 10, _stats["damage"]);

        base.Effect();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }
}