using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchBeing : Enemy
{
    int _dmg = 777;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _name = "Reaper";
        _agent.speed = 5.5f;
        _dmg = 15;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override protected void Act()
    {

    }

    override protected void Move()
    {

    }
}
