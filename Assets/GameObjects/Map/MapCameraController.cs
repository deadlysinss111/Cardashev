using System;
using System.Collections;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{

    Camera _camera;
    float _transitionSpeed;
    GameObject _targetNode;
    [NonSerialized] public Vector3 _baseMapPos;

    void Start()
    {
        _transitionSpeed = 2f;
        _camera = GetComponent<Camera>();
        SetCamPos(GI._mapPrefab.GetComponent<MapManager>()._playerLocation.transform.position, true);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
            ScrollCamera();

        // Transition the camera's position if the target node is valid
        if (_targetNode != null)
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _targetNode.transform.position + new Vector3(0, 10, -10), _transitionSpeed * Time.deltaTime);
    }

    public void UpdateNodeTarget(GameObject node)
    {
        _targetNode = node;
    }
    void ScrollCamera()
    {
        Vector3 pos = _camera.transform.position;
        if (Input.mouseScrollDelta.y < 0 && pos.z > -4.2f || Input.mouseScrollDelta.y > 0 && pos.z < 127.8f)
            _camera.transform.position = new Vector3(pos.x, pos.y, pos.z + Input.mouseScrollDelta.y * 2);
    }

    public void SetCamPos(Vector3 pos, bool addOffset)
    {
        _camera.transform.position = pos + (addOffset ? new Vector3(0, 15, -20) : new Vector3());
    }
}
