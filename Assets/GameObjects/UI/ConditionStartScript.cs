using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ConditionStartScript : MonoBehaviour
{
    Vector3 _activePos;
    Vector3 _inactivePos;

    private void Start()
    {
        _activePos = transform.localPosition;
        _inactivePos = transform.localPosition-new Vector3(450,0,0);

        transform.localPosition = _inactivePos;
    }

    // Update is called once per frame
    void Update()
    {
        print($"Update function");

        if (Time.timeScale < 1.0f )
            transform.localPosition = Vector3.Lerp(transform.localPosition, _activePos, 5*Time.unscaledDeltaTime);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, _inactivePos, 5*Time.unscaledDeltaTime);
    }
}
