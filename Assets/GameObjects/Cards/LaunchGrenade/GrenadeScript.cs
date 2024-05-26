using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public int _lifetime;
    void Awake()
    {
        //transform.position = GameObject.Find("Player").transform.position + Vector3.Scale(new Vector3(5.0f, 5.0f, 5.0f), GameObject.Find("Player").transform.rotation.eulerAngles);
        //GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(0, 0.2f, 0.1f), GameObject.Find("Player").transform.rotation.eulerAngles));
    }
}
