using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _damage;
    public float _velocity = 5;
    Vector3 _direction;
    Vector3 _lastPosition;

    void Start()
    {
        _lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider collider)
    {
        CollisionOccured(collider.gameObject);
    }

    void Update()
    {
        transform.position += _direction * _velocity * Time.deltaTime;

        RaycastHit hit;

        if(Physics.Raycast(_lastPosition, transform.position, out hit))
        {
            CollisionOccured(hit.transform.gameObject, true);
        }
    }

    void LateUpdate()
    {
        _lastPosition = transform.position;
    }

    void CollisionOccured(GameObject collider, bool tunel=false)
    {
        if (collider.CompareTag("Enemy"))
        {
            collider.GetComponent<Enemy>().TakeDamage(_damage);
            print($"hit an enemy: {collider.gameObject.name}, tunel: {tunel}");
            Destroy(gameObject);
            return;
        }

        // TODO : Add a case for obstacle collision
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
        transform.rotation = Quaternion.LookRotation(_direction);
    }

    public void SetInitialValues(Vector3 position, int velocity, int damage)
    {
        transform.position = position;
        _damage = damage;
        _velocity = velocity;
    }
}
