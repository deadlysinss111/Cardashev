using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnLeaveRoom : MonoBehaviour
{
    PlayerManager _manager;
    List<GameObject> _buttons;
    bool _left = false;


    private void Awake()
    {
        _buttons = new List<GameObject>();
        _manager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            GenerateRewards();
        }
    }

    void Leave()
    {
        SceneManager.LoadScene("MapNavigation");
    }

    void GenerateRewards()
    {
        if(!_left)
        {
            GenerateGolds();
            DisplayButtons();
            _left = true;
        }
    }

    GameObject GenerateItem()
    {
        UnityEngine.Object BUTTON = Resources.Load("ButtonPrefab");
        GameObject button = (GameObject)Instantiate(BUTTON);
        button.GetComponent<Button>().onClick.AddListener(() => { Destroy(button); });
        button.transform.SetParent(GameObject.Find("Canvas").transform, false);
        return button;
    }

    void GenerateGolds()
    {
        GameObject button = GenerateItem();
        int amount = UnityEngine.Random.Range(10, 20);
        button.GetComponent<Button>().onClick.AddListener(() => { _manager._goldAmount = amount; });
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(amount.ToString());
        _buttons.Add(button);
    }

    void GenerateBooster()
    {

    }

    void DisplayButtons()
    {
        int y = 300;
        foreach (GameObject button in _buttons)
        {
            button.transform.localPosition = new Vector3(0, y, 0);
            y -= 100;
        }
    }
}
