using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class JackHammer : MonoBehaviour
{
    Action _updateEffect;

    Dictionary<int, Vector3> _intToVector = new Dictionary<int, Vector3>()
    {
        {0, new Vector3(0, 0, 2) },
        {1, new Vector3(2, 0, 0) },
        {2, new Vector3(0, 0, -2) },
        {3, new Vector3(-2, 0, 0) },
    };

    private void Awake()
    {
        //_updateEffect = () => { };
        _updateEffect = Operating;
    }

    private void Update()
    {
        //_updateEffect();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.gameObject.CompareTag("TMTopology"))
        {
            _updateEffect();
            print("collided");
        }

    }

    void Operating()
    {
        ////gameObject.GetComponent<Rigidbody>().AddForce(0, 10, 1);
        //Physics.Raycast(transform.position + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hit);
        //Vector3 target = hit.transform.position + _intToVector[1];
        //Vector3 velocity = TrajectoryToolbox.BellCurveInitialVelocity(hit.transform.position, target, 5)*5;
        ////velocity.y *= 1000;
        //gameObject.GetComponent<Rigidbody>().AddForce(velocity);
        HierarchySearcher.FindChildRecursively(transform, "Animator").GetComponent<Animator>().SetTrigger("Bounce");
    }

    public void Move()
    {
        GetComponent<Rigidbody>().velocity += _intToVector[UnityEngine.Random.Range(0, 4)];
    }
}
