using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _damage;
    Vector3 _direction;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(_damage);
            Destroy(this);
        }
    }

    void Update()
    {
        transform.position += _direction;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
