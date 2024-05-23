using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchGrenade : Card
{
    public override void Effect()
    {
        base.Effect();

        Object grenade = Resources.Load("Grenade");
        Instantiate(grenade);
    }
}
