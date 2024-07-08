using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEffectScript : MonoBehaviour
{
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamageEffect()
    {
        _animator.SetTrigger("TakeDamage");
    }

    public void SetTimeStopTo(bool value)
    {
        _animator.SetBool("IsTimeStopped", value);
    }
}
