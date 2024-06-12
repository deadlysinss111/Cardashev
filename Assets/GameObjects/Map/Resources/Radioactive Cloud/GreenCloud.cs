using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCloud : MonoBehaviour
{
    // Particles to indicate movement
    [SerializeField] ParticleSystem _particleSystem;
    public AnimationCurve _moveCurve;
    float _animationTime;

    // Movement of the cloud
    public Vector3 _targetPosition;
    float _cloudSpeed;
    bool _cloudMoving;

    // After movement
    [SerializeField] LayerMask _layerMask;
    [SerializeField] GameObject _outerCloud;
    [SerializeField] GameObject _inerCloud;
    [SerializeField] GameObject _centerCloud;

    private void Start()
    {
        _cloudMoving = false;
        _animationTime = 0f;
        _cloudSpeed = .5f;
        _particleSystem.Stop();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, _targetPosition) > 3)
        {
            _animationTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCurve.Evaluate(_animationTime) * _cloudSpeed * Time.deltaTime);
        }
        else if (_cloudMoving)
        {
            _cloudMoving = false;
            ContaminateNodes();
        }
    }

    public void MoveCloudTo(Vector3 targetPosition)
    {
        _animationTime = 0;
        _cloudMoving = true;
        _targetPosition = targetPosition;
    }

    void ContaminateNodes()
    {
        Collider[] nodes = Physics.OverlapSphere(_outerCloud.transform.position, _outerCloud.transform.localScale.x/2, _layerMask);

        foreach (var item in nodes)
        {
            item.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
