using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dix : Idealist
{
    public Dix()
    {
        _name = "Dix";
        _baseHP = 80;
        _instance = this;
        _startingDeck = new List<string> 
        {
            "LaunchGrenadeCard",
            "PiercingShotCard",
            "SimpleShotCard",
            "JumpCard",
            "JumpAndShockwaveCard"
            //"OverdriveCard",
            //"CoverCard",
        };
    }
    
    public override void Ultimate()
    {

    }
}
