using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


// Will be Inherited by every specific .cs descirbing actual characters
// This is pretty empty since they need to share few things
static public class Idealist
{
    static public string _name = "brawler";
    static public byte _HP;
    static public Action _ultimate;
}