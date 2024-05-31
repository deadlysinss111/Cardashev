using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class RoomTimer : MonoBehaviour
{
    private float _timeSpentInRoom = 0f;
    private bool _isPlayerInRoom = false;
    [SerializeField] private Vector3 _roomSize;
    [SerializeField] private string _roomID = "-1"; // Unique identifier for the room

    private BoxCollider _box;
    private Rigidbody _rb;

    private void Awake()
    {
        _box = GetComponent<BoxCollider>();
        _rb = GetComponent<Rigidbody>();

        _box.isTrigger = true;
        _box.size = _roomSize;

        _rb.isKinematic = true;
    }

    private void Update()
    {
        if (_isPlayerInRoom)
        {
            _timeSpentInRoom += Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _roomSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInRoom = true;
            RoomManager._Instance.EnterRoom(_roomID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRoom = false;
            RoomManager._Instance.ExitRoom(_roomID, _timeSpentInRoom);

            // Reset the timer if the roomID is -1
            if (_roomID == "-1")
            {
                _timeSpentInRoom = 0f;
            }
        }
    }

    public float GetTimeSpentInRoom()
    {
        return _timeSpentInRoom;
    }
}
