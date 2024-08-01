using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dix : Idealist
{
    public Dix()
    {
        _name = "Dix";
        _baseHP = 100;
        _instance = this;
        _startingDeck = new List<string> 
        {
            "SimpleShotCard",
            "SimpleShotCard",
            "SimpleShotCard",
            "SimpleShotCard",
            "LaunchGrenadeCard",
            "LaunchGrenadeCard",
            "JumpCard",
            //"ResilienceCard",
            //"LaunchGrenadeCard",
            //"PiercingShotCard",
            //"JumpAndShockwaveCard",
            "OverdriveCard",
            //"SecondSleeve"
            //"CoverCard",
        };
    }
    
    public override void Ultimate()
    {

    }
}
