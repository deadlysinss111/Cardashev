using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _damage;
    Vector3 _direction;
    float _velocity = 3;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(_damage);
            Destroy(this);
        }

        // TODO : Add a case for obstacle collision
    }

    void Update()
    {
        transform.position += _direction * _velocity * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
        transform.rotation = Quaternion.LookRotation(_direction);
    }

    public void SetInitialPosition(Vector3 position)
    {
        transform.position = position;
    }
}
