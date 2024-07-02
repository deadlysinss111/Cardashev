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
        GetComponentInChildren<Image>().color = Color.red;
        _animator.SetTrigger("TakeDamage");
    }

    public void SetTimeStopTo(bool value)
    {
        GetComponentInChildren<Image>().color = Color.cyan;
        _animator.SetBool("IsTimeStopped", value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TakeDamageEffect();
        if (Input.GetKeyDown(KeyCode.Y))
            SetTimeStopTo(false);
    }
}
