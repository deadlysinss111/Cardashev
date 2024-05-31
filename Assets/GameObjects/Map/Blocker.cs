using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField] private float time;

    public GameObject _door1;
    public GameObject _door2;

    private float _startDoor1X;
    private float _endDoor1X;

    private float _startDoor2X;
    private float _endDoor2X;

    // Start is called before the first frame update
    void Start()
    {
        _startDoor1X = _door1.transform.localPosition.x;
        _endDoor1X = -1.182f;

        _startDoor2X = _door2.transform.localPosition.x;
        _endDoor2X = 0.394f;
        BetterDebug.Log(_startDoor1X, _endDoor1X, _startDoor2X, _endDoor2X);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _door1.transform.localPosition;
        pos.x = Mathf.Lerp(_startDoor1X, _endDoor1X, Mathf.Clamp01(GlobalInformations._gameTimer/time));
        _door1.transform.localPosition = pos;

        pos = _door2.transform.localPosition;
        pos.x = Mathf.Lerp(_startDoor2X, _endDoor2X, Mathf.Clamp01(GlobalInformations._gameTimer / time));
        _door2.transform.localPosition = pos;
    }

    private void FixedUpdate()
    {
        GlobalInformations._gameTimer += Time.deltaTime;
    }
}
