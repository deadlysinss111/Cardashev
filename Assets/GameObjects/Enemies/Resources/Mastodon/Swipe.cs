using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Swipe : MonoBehaviour
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
        if(collider.gameObject.TryGetComponent<Enemy>(out _))return;

        if(collider.gameObject.TryGetComponent<StatManager>(out StatManager target))
        {
            target.TakeDamage(_dmg);
        }
    }
}
