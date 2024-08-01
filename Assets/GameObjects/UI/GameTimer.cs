using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float _timePassedInRoom = 0.0f;

    TMPro.TextMeshProUGUI _roomTimer;
    TMPro.TextMeshProUGUI _runTimer;

    void Start()
    {
        // Fetching the 2 GO containing the text timer so we can update them later
        _roomTimer = HierarchySearcher.FindChild(transform, "RoomTimer").GetComponent<TMPro.TextMeshProUGUI>();
        _runTimer = HierarchySearcher.FindChild(transform, "GlobalTimer").GetComponent<TMPro.TextMeshProUGUI>();

        Debug.Log("_roomTimer : " + _roomTimer.name);
        Debug.Log("_runTimer : " + _runTimer.name);
    }

    private void OnDestroy()
    {
        GI._lastRoomTimer = _timePassedInRoom;
    }

    void Update()
    {
        // Updates the time passed in the current run
        _timePassedInRoom += Time.deltaTime;
        GI._gameTimer += Time.deltaTime;

        // Prints it out on the HUD
        _roomTimer.text = GetFormattedTime(_timePassedInRoom);
        _runTimer.text = GetFormattedTime(GI._gameTimer);
    }

    static public string GetFormattedTime(float ARGtimeToFormat)
    {
        int minutes = Mathf.FloorToInt(ARGtimeToFormat / 60);
        int seconds = Mathf.FloorToInt(ARGtimeToFormat % 60);

        return $"{minutes:D2}:{seconds:D2}";
    }
}
