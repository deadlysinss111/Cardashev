using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private float _limit;

    public GameObject _door1;
    public GameObject _door2;

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = new(Mathf.Clamp01(GlobalInformations._gameTimer / _time)*_limit, 1, 1);

        _door1.transform.localScale = scale;
        _door2.transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        //GlobalInformations._gameTimer += Time.deltaTime;
    }
}
