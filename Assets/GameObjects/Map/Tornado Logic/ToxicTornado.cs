using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToxicTornado : DynamicMapObj
{
    /*
     FIELDS
    */
    // Particles to indicate movement
    public AnimationCurve _moveCurve;
    float _animationTime;

    // Movement of the cloud
    public Vector3 _targetPosition;
    float _cloudSpeed;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] Wind _wind;

    int _xMin = -16;
    int _xMax = 50;
    int _zMin = 20;
    int _zMax = 90;

    Vector3 _direction;
    float _xSurplus = 0;
    float _zSurplus = 0;

    Vector3 _desiredPos;

    /*
     METHODS
    */
    private new void Awake()
    {
        // Essential event subscribing to update
        base.Awake();

        _desiredPos = transform.position;

        _direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        _direction = Vector3.Normalize(_direction);

        transform.position = new Vector3(UnityEngine.Random.Range(_xMin, _xMax), transform.position.y, UnityEngine.Random.Range(_zMin, _zMax));
    }

    private void Update()
    {
        //if(Input.anyKeyDown)
        //{
        //    GI._UeOnMapSceneLoad.Invoke();
        //}
        _desiredPos.y = transform.position.y;
        
    }

    IEnumerator WaitBeforeMoving()
    {
        float offset = 1.6f;
        while(offset > 0)
        {
            offset-= Time.deltaTime;
            yield return null;
        }
        while (transform.position != _desiredPos)
        {
            transform.position = Vector3.Lerp(transform.position, _desiredPos, 0.1f);
        }
    }


    // GreenCloud's update on Map load
    protected override void UpdDynamicMapObj()
    {
        do
        {
            _direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            _direction = Vector3.Normalize(_direction);
        } while (CheckForBoundaries(_desiredPos + 10 * _direction));
        
        Move();
        _wind.SetToDirection(_direction);
    }

    // ------
    // GREENCLOUD SPECIFIC EFFECTS
    // ------

    void Move()
    {
        // TODO: tweak that 0.6f for balancing
        //transform.Translate(_direction * GI._lastRoomTimer * 0.6f);
        _desiredPos = transform.position + _direction * GI._lastRoomTimer * 0.6f;
        print(_desiredPos);
        //transform.Translate(_direction * 10);
        CheckForBoundaries();
        print(_desiredPos);
    }

    //void MoveSurplus()
    //{
    //    transform.Translate(Vector3.Scale(_direction, new Vector3(_xSurplus, 0, _zSurplus)));
    //    _xSurplus = 0;
    //    _zSurplus = 0;
    //    CheckForBoundaries();
    //}

    //void InvertDirection()
    //{
    //    _direction = -_direction;
    //    _direction += new Vector3(UnityEngine.Random.Range(-0.6f, 0.6f), 0, UnityEngine.Random.Range(-0.6f, 0.6f));
    //    _direction = Vector3.Normalize(_direction);
    //}

    void CheckForBoundaries()
    {
        if(_desiredPos.x < _xMin)
        {
            //_xSurplus = _xMin - transform.position.x;
            _desiredPos = new Vector3(_xMin, transform.position.y, transform.position.z);
        }
        else if(_desiredPos.x > _xMax)
        {
            //_xSurplus -= _xMax - transform.position.x;
            _desiredPos = new Vector3(_xMax, transform.position.y, transform.position.z);
        }

        if(_desiredPos.z < _zMin)
        {
            //_zSurplus = _zMin - transform.position.z;
            _desiredPos = new Vector3(transform.position.x, transform.position.y, _zMin);
        }
        else if(_desiredPos.z > _zMax)
        {
            //_zSurplus = _zMax - transform.position.z;
            _desiredPos = new Vector3(transform.position.x, transform.position.y, _zMax);
        }

        //if (_xSurplus > 0 || _zSurplus > 0)
        //{
        //    InvertDirection();
        //    MoveSurplus();
        //}
    }

    // This override returns true if the target is out of the boundaries
    bool CheckForBoundaries(Vector3 target)
    {
        if (target.x < _xMin)
        {
            return true;
        }
        else if (target.x > _xMax)
        {
            return true;
        }

        if (target.z < _zMin)
        {
            return true;
        }
        else if (target.z > _zMax)
        {
            return true;
        }

        return false;
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Map Node"))
        {
            other.gameObject.GetComponent<MapNode>().Contaminate(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Map Node"))
        {
            other.gameObject.GetComponent<MapNode>().Contaminate(false);
        }
    }
}
