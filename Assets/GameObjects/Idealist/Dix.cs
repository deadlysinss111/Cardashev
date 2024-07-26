using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dix : Idealist
{
    public Dix()
    {
        _name = "Dix";
        _baseHP = 160;
        _instance = this;
        _startingDeck = new List<string> 
        {
            "SimpleShotCard",
            "SimpleShotCard",
            "SimpleShotCard",
            "SimpleShotCard",
            "SimpleShotCard",
            "JumpCard",
            "JumpCard",
            "JumpCard",
            "ResilienceCard",
            "ResilienceCard",
            //"LaunchGrenadeCard",
            //"PiercingShotCard",
            //"JumpAndShockwaveCard",
            //"OverdriveCard",
            //"SecondSleeve"
            //"CoverCard",
        };
    }
    
    public override void Ultimate()
    {

    }
}
