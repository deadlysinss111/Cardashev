using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PiercingShot : Card
{
    List<GameObject> _selectableTiles = new List<GameObject>();
    Vector3 _direction;
    Vector3 _startingPosition;
    int _damage;

    void Awake()
    {
        _duration = 4f;
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"damage", 8},
            {"critDamage", 18 },
            {"speed", 20 },
            {"critSpeed", 100 },
        };
        base.Init("PiercingShot", 3, 4, 85, stats, $"Shoot a powerful bullet dealing {stats["damage"]} dmg. Critical: Damage and speed increased.");

        while (PlayerManager.AddState("PiercingShot" + _id.ToString(), EnterState, ExitState) == false) _id++;

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
        manager.SetHoverTo(Preview);
        manager.SetWallsAsClickable(true);
        GI.UpdateCursorsInverted("Bow", (byte)(GI.CursorRestriction.S_TILES));
    }

    void LeftClick()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetToDefault();

        if (manager._lastHit.transform == null) return;

        _direction = Vector3.Normalize(manager._lastHit.transform.position - manager._virtualPos);
        _direction.y = 0;
        _startingPosition = manager._virtualPos;

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

        if (GI._PManFetcher()._statManagerRef._isCriting)
        {
            bullet.GetComponent<Bullet>().SetInitialValues(_startingPosition, _stats["critSpeed"], _stats["critDamage"], 1.5f);
        }
        else
        {
            bullet.GetComponent<Bullet>().SetInitialValues(_startingPosition, _stats["speed"], _stats["damage"], 1.5f);
        }
        base.Effect();
    }

    IEnumerator Shoot()
    {
        bool offset = true;
        while (offset)
        {
            yield return new WaitForSeconds(1.5f);
            offset = true;
        }

    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    /*public override void Effect()
    {
        if (_target == null) return;
        base.Effect();
        _target.GetComponent<Enemy>().TakeDamage(_stats[GI._PlayerFetcher().GetComponent<StatManager>().HasCritical() ? 1 : 0]);
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        _selectableArea.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        _selectableArea.SetSelectableEntites(false, false, true, false);
        _selectableTiles = _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 7);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(() => {
            if (_selectableArea.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                if (obj.TryGetComponent(out Enemy _) == false)
                {
                    throw new MissingComponentException($"The object {obj.name} ({obj.GetType()}) the card aimed at does not have a Enemy script.");
                }
                _target = obj;
                GI._PlayerFetcher().GetComponent<PlayerManager>().SetToDefault();
                base.PlayCard();
            }
            else
                print("R u ok?");
        });
        manager.SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        manager.SetHoverTo(() => { });
        GI.UpdateCursors("Bow", (byte)GI.CursorRestriction.S_ENEMIES);
        GI.UpdateCursorsInverted("Cross", (byte)GI.CursorRestriction.S_ENEMIES);
    }
    
    void ExitState()
    {
        _selectableArea.ResetSelectable();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }*/
}