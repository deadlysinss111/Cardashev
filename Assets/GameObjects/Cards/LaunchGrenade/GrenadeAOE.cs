using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAOE : MonoBehaviour
{
    float _lifetime = 0.5f;

    private void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0)
            Destroy(gameObject);
    }
}
