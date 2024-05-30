using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float _timePassed;

    [SerializeField] private GameObject _timerText;

    void Start()
    {
        _timePassed = 0;
    }

    void Update()
    {
        _timePassed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(_timePassed / 60);
        int seconds = Mathf.FloorToInt(_timePassed % 60);

        _timerText.GetComponent<TMPro.TextMeshProUGUI>().text = $"{minutes:D2}:{seconds:D2}";

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            GameOverManager.Instance.StartGameOver();
        }
    }
}
