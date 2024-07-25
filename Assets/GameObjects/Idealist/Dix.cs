using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dix : Idealist
{
    public Dix()
    {
        _name = "Dix";
        _baseHP = 6000;//put it back to 6000 when done testion GOver
        _instance = this;
        _startingDeck = new List<string> 
        {
            "LaunchGrenadeCard",
            "PiercingShotCard",
            "SimpleShotCard",
            "JumpCard",
            "JumpAndShockwaveCard",
            "OverdriveCard",
            "ResilienceCard",
            //"SecondSleeve"
            //"CoverCard",
        };
    }
    
    public override void Ultimate()
    {

    }
}
