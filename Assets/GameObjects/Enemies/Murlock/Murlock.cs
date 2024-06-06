using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murlock : Enemy
{
    [SerializeField] GameObject _swipeZone;

    override protected void Act()
    {
        
    }

    private void Swipe()
    {
        Vector3 target = _swipeZone.transform.position;
        Vector3 dir = Vector3.Normalize(target - transform.position);
        //if ()
        //{
            
        //}
        //Instantiate(_swipeZone, transform.position, transform.);
    }
}
