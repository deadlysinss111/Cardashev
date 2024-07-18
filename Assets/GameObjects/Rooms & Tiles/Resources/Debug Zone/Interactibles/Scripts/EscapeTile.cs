using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeTile : Interactible
{
    // Contains name of the condition (such as enemy name) and amount of time this condition must be resolved to clear the room
    [SerializeField] Dictionary<string, byte> _conditions = new();

    [SerializeField] GameObject _rewardIndicator;
    CanvasGroup _rewardCanvas;

    [SerializeField] TextMeshProUGUI _runText;
    [SerializeField] TextMeshProUGUI _condText;
    [SerializeField] TextMeshProUGUI _rewardText;

    [SerializeField] Animator _animator;
    bool _rewardScreenOn = false;

    Action OnWalk = () => {
        GI._PlayerFetcher().GetComponent<DeckManager>().UnloadDeck();
        GI._loader.LoadScene("Room", "Map");
        GI.ResetCursorValues();
    };

    private void Start()
    {
        _rewardCanvas = _rewardIndicator.GetComponentInParent<CanvasGroup>();
    }

    private new void Awake()
    {
        base.Awake();
        _rewardIndicator.GetComponentInParent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        _rewardIndicator.GetComponentInParent<Canvas>().worldCamera = GameObject.FindAnyObjectByType<Camera>();
        _rewardIndicator.GetComponentInParent<Canvas>().renderMode = RenderMode.WorldSpace;

        RectTransform rect = _rewardIndicator.transform.parent.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0f, 3.72f, 0);
        rect.sizeDelta = new Vector2(1145, 514);
        //rect.offsetMax = new Vector2(-550, -232);
        rect.localRotation = Quaternion.Euler(0, -90, 0);
        rect.localScale = new Vector3(0.1f, 0.1f, 1);

        RegenerateConditionText();
        _animator.Play("PopsOut");

        //_rewardCanvas.alpha = 0f;
    }

    private void Update()
    {
        float dist = Vector3.Distance(transform.position, GI._PlayerFetcher().transform.position);
        if (dist < 20 && _rewardScreenOn == false)
        {
            _rewardScreenOn = true;
            _animator.Play("PopsIn");
        }
        else if (dist >= 20 && _rewardScreenOn)
        {
            _rewardScreenOn = false;
            _animator.Play("PopsOut");
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            OnWalk();
    }

    // You can either add a condition trough code, or set conditions by SerializedField (Dictionaries are not serialized)
    public void AddCondition(string name, byte amount = 1)
    {
        if (_conditions.ContainsKey(name))
            _conditions[name] += amount;
        else
            _conditions.Add(name, amount);
        RegenerateConditionText();
    }

    // We must call this function whene a condition triggers (such as enemy death)
    public void TriggerCondition(string name)
    {
        if(name != null)
        {
            if (_conditions.ContainsKey(name))
            {
                if(_conditions[name] > 0)
                    _conditions[name] -= 1;
            }
        }
        foreach (byte amount in _conditions.Values)
        {
            if (amount > 0)
                return;
        }

        RegenerateConditionText();
        LevelCleared();
    }

    // Whene all conditions are resolved
    void LevelCleared()
    {
        // TODO: change sprite or smth
        // & trigger rewards
        Reward._content = new Reward.Content(10, 20, 0);
        gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.green;
        OnWalk = ()=> {
            GI._PlayerFetcher().GetComponent<DeckManager>().UnloadDeck();
            GI._loader.LoadScene("Room", "Reward"); 
        };
        _runText.text = "[Go on]";
        _rewardText.text = "Rewards: [OK]";
    }

    void RegenerateConditionText()
    {
        _condText.text = "Reward Conditions:\n";
        foreach (KeyValuePair<string, byte> cond in _conditions)
        {
            _condText.text += $"- {cond.Key} (${cond.Value} left)\n";
        }
    }
}
