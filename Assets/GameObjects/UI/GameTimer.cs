using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float _timePassed;


    GameObject _roomTimer;
    GameObject _globalTimer;

    void Start()
    {
        _roomTimer = GameObject.Find("RoomTimer");
        _globalTimer = GameObject.Find("GlobalTimer");
        _timePassed = 0;
    }

    private void OnDestroy()
    {
        GI._lastRoomTimer = _timePassed;
    }

    void Update()
    {
        // Stops updating if we are on the end screen
        //if (GameOverManager._instance._inGameOver || _winScreen.GetComponent<RoomClear>()._roomClearScreen) return;

        // Updates the time passed in the current run
        float ratilo = Time.deltaTime;
        _timePassed += ratilo;
        GI._gameTimer += ratilo / 2.0f;

        // Updates both timers
        _roomTimer.GetComponent<TMPro.TextMeshProUGUI>().text = GetFormattedTime(_timePassed);
        _globalTimer.GetComponent<TMPro.TextMeshProUGUI>().text = GetFormattedTime(GI._gameTimer);
    }

    static public string GetFormattedTime(float ARGtime)
    {
        int minutes = Mathf.FloorToInt(ARGtime / 60);
        int seconds = Mathf.FloorToInt(ARGtime % 60);

        return $"{minutes:D2}:{seconds:D2}";
    }
}
