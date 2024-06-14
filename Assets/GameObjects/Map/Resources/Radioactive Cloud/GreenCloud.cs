using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCloud : DynamicMapObj
{
    /*
     FIELDS
    */
    // Particles to indicate movement
    [SerializeField] ParticleSystem _particleSystem;
    public AnimationCurve _moveCurve;
    float _animationTime;

    // Movement of the cloud
    public Vector3 _targetPosition;
    float _cloudSpeed;

    // After movement
    [SerializeField] LayerMask _layerMask;
    [SerializeField] GameObject _outerCloud;
    [SerializeField] GameObject _inerCloud;
    [SerializeField] GameObject _centerCloud;

    /*
     METHODS
    */
    private new void Awake()
    {
        // Essential event subscribing to update
        base.Awake();

        _animationTime = 0.0f;
        _cloudSpeed = .5f;
        _particleSystem.Stop();
    }


    // GreenCloud's update on Map load
    protected override void UpdDynamicMapObj()
    {
        StartCoroutine(MoveCloudTo());
    }

    IEnumerator MoveCloudTo()
    {
        // While the cloud hasn't arrived, move it towards the target
        while (Vector3.Distance(this.transform.position, _targetPosition) > 3)
        {
            _animationTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCurve.Evaluate(_animationTime) * _cloudSpeed * Time.deltaTime);
            yield return null;
        }

        // Once we arrived, contaminate nodes
        ContaminateNodes();
    }

    // ------
    // GREENCLOUD SPECIFIC EFFECTS
    // ------

    public void UpdTarget(Vector3 targetPosition)
    {
        _animationTime = 0.0f;
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
