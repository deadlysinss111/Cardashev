using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager _Instance;

    private Dictionary<string, float> _roomTimes = new Dictionary<string, float>();

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterRoom(string roomID)
    {
        if (!_roomTimes.ContainsKey(roomID))
        {
            _roomTimes[roomID] = 0f;
        }
        Debug.Log("Player entered room: " + roomID);
    }

    public void ExitRoom(string roomID, float timeSpent)
    {
        if (_roomTimes.ContainsKey(roomID))
        {
            _roomTimes[roomID] += timeSpent;
        }
        else
        {
            _roomTimes[roomID] = timeSpent;
        }
        Debug.Log("Player exited room: " + roomID + ". Total time spent: " + _roomTimes[roomID] + " seconds");
    }

    public float GetTotalTimeSpentInRoom(string roomID)
    {
        if (_roomTimes.ContainsKey(roomID))
        {
            return _roomTimes[roomID];
        }
        else if (_roomTimes.ContainsKey(roomID) == false)
        {
            return _roomTimes[roomID] = 0f;
        }
        return 0f;
    }
}
