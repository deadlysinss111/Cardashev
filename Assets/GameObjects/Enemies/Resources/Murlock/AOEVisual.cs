using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AOEVisual : MonoBehaviour
{
    float _lifeTime = 0.5f;
    public int _dmg;
    public GameObject _originEnemy;

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
        if (collider.gameObject == _originEnemy) return;

        if(collider.gameObject.TryGetComponent(out StatManager target))
        {
            if (collider.gameObject.TryGetComponent(out CoverManager cover) && cover._activated)
            {
                cover._health -= _dmg;
            }
            else
            {
                target.TakeDamage(_dmg);
            }
        }
    }
}
