using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public int _damage;
    public float _velocity = 5;
    Vector3 _direction;
    Vector3 _lastPosition;
    float _lifetime;

    // Those 3 are meant for the offset stuff which is temporary
    float _offsetTime = 0;
    Vector3 _launchPos;
    bool _readyToGo = false;
    

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
        if(_offsetTime > 0)
        {
            _offsetTime -= Time.deltaTime;
            return;
        }
        if (_readyToGo)
        {
            transform.position = _launchPos;
            _readyToGo = false;
        }

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

            //collider.GetComponent<MeshRenderer>().material.color = Color.red;
            
            Destroy(gameObject);
            return;
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
        transform.rotation = Quaternion.LookRotation(_direction);
    }

    public void SetInitialValues(Vector3 position, int velocity, int damage, float offset = 0)
    {
        _damage = damage;
        _velocity = velocity;
        _offsetTime = offset;
        _launchPos = position;
        if(_offsetTime > 0)
        {
            _readyToGo = true;
            transform.position = position + new Vector3(0, 4, 0);
        }
        else
            transform.position = position;
    }
}
