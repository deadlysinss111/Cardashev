using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

// DEBUG CARD
// Because I got bored of waiting for another card beside the grenade one
class Jeku : Card
{
    private void Start()
    {
        _name = "Jeku Card";
        _description = "Maybe a misspelled version of the Joker Card. Due to its wrong name, it has no effect.";
        _duration = 10f;

        _maxLv = 4;
    }
}