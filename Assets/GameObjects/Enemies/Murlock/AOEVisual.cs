using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AOEVisual : MonoBehaviour
{
    float _lifeTime = 0.5f;
    public int _dmg;

    private void Update()
    {
        _lifeTime -= Time.deltaTime;
        if( _lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        StatManager target;
        if(collider.gameObject.TryGetComponent<StatManager>(out target))
        {
            target.TakeDamage(_dmg);
            print("ye");
        }
    }
}
