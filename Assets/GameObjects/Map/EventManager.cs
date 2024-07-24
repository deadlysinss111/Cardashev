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
        }

        public string name;

        [TextArea] public string description;
        public EventChoice[] choices;
    }

    public RandomEvent[] events;
    TextMeshProUGUI title;
    TextMeshProUGUI description;
    GameObject choicesList;
    List<GameObject> currentChoices;
    [SerializeField] GameObject CHOICE_BUTTON;
    Image splashArt;

    public void StartEvent()
    {
        title = GameObject.Find("Event Title").GetComponent<TextMeshProUGUI>();
        description = GameObject.Find("Event Description").GetComponent<TextMeshProUGUI>();
        choicesList = GameObject.Find("Choices List");
        splashArt = GameObject.Find("Event Splash Art").GetComponent<Image>();

        GameObject.Find("Canvas").GetComponent<Animator>().SetTrigger("Slide In");
        RandomEvent e = events[Random.Range(0, events.Length)];

        title.text = e.name;
        description.text = e.description;
        // splashArt = e.
        for (int i = 0; i < e.choices.Length; i++)
        {
            GameObject button = Instantiate(CHOICE_BUTTON, choicesList.transform);
            currentChoices.Add(button);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-300 + 300*i, 38, 0);
            button.GetComponentInChildren<TextMeshProUGUI>().text = e.choices[i].name;
            UnityEvent func = e.choices[i].result;
            button.GetComponent<Button>().onClick.AddListener(() => { func?.Invoke(); });
            if (e.choices[i].subsequentChoices.Length > 0)
            {
                EventChoice temp = e.choices[i];
                button.GetComponent<Button>().onClick.AddListener(() => { NextPrompt(temp); });
            }

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
    }

    void NextPrompt(EventChoice eventChoice)
    {
        foreach (GameObject button in currentChoices)
        {
            Destroy(button);
        }
        currentChoices.Clear();
    }

    public void Debug(string msg)
    {
        print(msg);
    }
}
