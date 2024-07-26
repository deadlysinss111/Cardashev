using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EventManager.RandomEvent;

public class EventManager : MonoBehaviour
{
    [System.Serializable] public class RandomEvent // Class for storing all the event's data.
    {
        [System.Serializable] public class EventChoice // Class for storing the UnityEvent assigned to the choice.
        {
            public string name;
            public UnityEvent result;
            [TextArea] public string description;
            public EventChoice[] subsequentChoices = new EventChoice[0];

            [Range(0,100)] public int succesPercentage;
            public EventChoice[] success;
            public EventChoice[] fail;
        }

        public string name;

        public bool isActive;

        [TextArea] public string description;
        public EventChoice[] choices;
    }

    public RandomEvent[] events;
    List<RandomEvent> availableEvents;
    TextMeshProUGUI title;
    TextMeshProUGUI description;
    GameObject choicesList;
    List<GameObject> currentChoices;
    [SerializeField] GameObject CHOICE_BUTTON;
    Image splashArt;

    private void Awake()
    {
        foreach (var item in events)
        {
            if (item.isActive)
                availableEvents.Add(item);
        }
    }

    public void StartEvent()
    {
        currentChoices = new List<GameObject>();

        title = GameObject.Find("Event Title").GetComponent<TextMeshProUGUI>();
        description = GameObject.Find("Event Description").GetComponent<TextMeshProUGUI>();
        choicesList = GameObject.Find("Choices List");
        splashArt = GameObject.Find("Event Splash Art").GetComponent<Image>();

        GameObject.Find("Canvas").GetComponent<Animator>().SetTrigger("Slide In");
        RandomEvent e = availableEvents[Random.Range(0, events.Length)];

        title.text = e.name;
        description.text = e.description;
        // splashArt = e.
        AddChoices(e.choices);
        
    }

    void AddChoices(EventChoice[] choices)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            GameObject button = Instantiate(CHOICE_BUTTON, choicesList.transform);
            currentChoices.Add(button);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-300 + 300 * i, 38, 0);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choices[i].name;
            UnityEvent func = choices[i].result;
            button.GetComponent<Button>().onClick.AddListener(() => { func?.Invoke(); });
            if (choices[i].succesPercentage > 0)
            {
                int successPerc = choices[i].succesPercentage;
                EventChoice success = choices[i].success[0];
                EventChoice fail = choices[i].fail[0];
                button.GetComponent<Button>().onClick.AddListener(() => { RandomBooleanSuccess(successPerc, success, fail); });
            }
            else if (choices[i].subsequentChoices.Length > 0)
            {
                EventChoice temp = choices[i];
                button.GetComponent<Button>().onClick.AddListener(() => { NextPrompt(temp); });
            }
        }
    }

    void NextPrompt(EventChoice eventChoice)
    {
        foreach (GameObject button in currentChoices)
        {
            Destroy(button);
        }
        currentChoices.Clear();

        title.text = eventChoice.name;
        description.text = eventChoice.description;
        AddChoices(eventChoice.subsequentChoices);
    }

    public void RandomBooleanSuccess(int successPerc, EventChoice successResult, EventChoice failResult)
    {
        if (Random.Range(1, 101) < successPerc)
        {
            // Successful result
            NextPrompt(successResult);
            successResult.result.Invoke();
        }
        else
        {
            // Failed result
            NextPrompt(failResult);
            failResult.result.Invoke();
        }
    }

    // <========== Event Function Library ==========> //
    // Here you can code any function that would be used by an event that doesn't fit in any class in particular.

    public void ChangeGoldAmount(int amount)
    {
        CurrentRunInformations._goldAmount += amount;
    }

    public void HealPlayer(int amount)
    {
        GI._PManFetcher()._statManagerRef.Heal(amount);
    }

    public void DamagePlayer(int amount)
    {
        GI._PManFetcher()._statManagerRef.TakeDamage(amount);
    }

    public void CloseEvent()
    {
        GameObject.Find("Canvas").GetComponent<Animator>().SetTrigger("Slide Out");

        GI._canClickOnNode = true;
    }
}
