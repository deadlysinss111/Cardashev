using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Camera _camera;
    float _transitionSpeed;
    GameObject _targetNode;

    // Start is called before the first frame update
    void Start()
    {
        _transitionSpeed = 2f;
    }

    private void Update()
    {
        if (_targetNode != null)
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _targetNode.transform.position + new Vector3(0, 10, -10), _transitionSpeed * Time.deltaTime);
    }

    public void MoveCamToNode(GameObject node)
    {
        _targetNode = node;
    }

    public void SetCamPos(Vector3 pos, bool addOffset)
    {
        _camera.transform.position = pos + (addOffset ? new Vector3(0, 15, -20) : new Vector3());
    }
}
