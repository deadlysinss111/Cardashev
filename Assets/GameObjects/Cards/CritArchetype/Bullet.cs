using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _damage;
    public float _velocity = 5;
    Vector3 _direction;
    Vector3 _lastPosition;
    float _lifetime;

    void Start()
    {
        _lastPosition = transform.position;
        _lifetime = 10;
    }

    void OnTriggerEnter(Collider collider)
    {
        CollisionOccured(collider.gameObject);
    }

    void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0) Destroy(gameObject);

        transform.position += _direction * _velocity * Time.deltaTime;

        RaycastHit hit;

        Debug.DrawRay(_lastPosition, transform.position - _lastPosition);
        if(Physics.Raycast(_lastPosition, transform.position - _lastPosition, out hit, _velocity * Time.deltaTime))
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
            print($"hit an Enemy: {collider.gameObject.name}, tunel: {tunel}");
            Destroy(gameObject);
            return;
        }

        // TODO : Add a case for obstacle collision
        if (collider.CompareTag("TMTopology") && collider.layer == LayerMask.NameToLayer("Wall"))
        {
            print($"hit an Obstacle: {collider.gameObject.name}, tunel: {tunel}");

            collider.GetComponent<MeshRenderer>().material.color = Color.red;
            
            Destroy(gameObject);
            return;
        }
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
