using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float _timePassed;

    [SerializeField] private GameObject _timerText;

    [SerializeField] private GameObject _winScreen;

    void Start()
    {
        _timePassed = 0;
    }

    private void OnDestroy()
    {
        GI._gameTimer += _timePassed;
    }

    void Update()
    {
        if (GameOverManager.Instance._inGameOver || _winScreen.GetComponent<WinManager>()._onWinScreen) return;

        _timePassed += Time.deltaTime;

        _timerText.GetComponent<TMPro.TextMeshProUGUI>().text = GetFormattedTime();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            GameOverManager.Instance.StartGameOver();
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
