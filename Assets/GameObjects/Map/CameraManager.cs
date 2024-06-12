using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Camera _camera;
    float _transitionSpeed;
    GameObject _targetNode;

    void Start()
    {
        _transitionSpeed = 2f;
    }

    private void Update()
    {
        // Transition the camera's position if the target node is valid
        if (_targetNode != null)
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _targetNode.transform.position + new Vector3(0, 10, -10), _transitionSpeed * Time.deltaTime);
    }

    public void UpdateNodeTarget(GameObject node)
    {
        _targetNode = node;
    }

    public void SetCamPos(Vector3 pos, bool addOffset)
    {
        _camera.transform.position = pos + (addOffset ? new Vector3(0, 20, -20) : new Vector3());
    }
}
