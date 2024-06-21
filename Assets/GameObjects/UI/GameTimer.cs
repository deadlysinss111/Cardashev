using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float _timePassed;

    [SerializeField] GameObject _timerText;
    [SerializeField] GameObject _winScreen;

    void Start()
    {
        _timePassed = 0;
        Debug.Log("GameTimer started");
    }

    private void OnDestroy()
    {
        GI._gameTimer += _timePassed;
    }

    void Update()
    {
        // Stops updating if we are on the end screen
        //if (GameOverManager._instance._inGameOver || _winScreen.GetComponent<RoomClear>()._roomClearScreen) return;

        // Updates the time passed in the current run
        _timePassed += Time.deltaTime;
        _timerText.GetComponent<TMPro.TextMeshProUGUI>().text = GetFormattedTime();

        // Debug trigger of the GameOver and Win screen
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            GameOverManager._instance.StartGameOver();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
        {
            _winScreen.SetActive(true);
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(_timePassed / 60);
        int seconds = Mathf.FloorToInt(_timePassed % 60);

        return $"{minutes:D2}:{seconds:D2}";
    }
}
