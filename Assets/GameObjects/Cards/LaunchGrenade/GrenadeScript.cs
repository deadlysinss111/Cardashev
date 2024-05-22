using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public int _lifetime;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.2f, 0.1f));
    }

    void Update()
    {
        
    }
}
