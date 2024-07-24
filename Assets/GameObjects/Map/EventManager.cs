using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [System.Serializable] public class RandomEvent // Class for storing all the event's data.
    {
        [System.Serializable] public class EventChoice // Class for storing the UnityEvent assigned to the choice.
        {
            public string name;
            public UnityEvent result;
            [TextArea] public string description;
            public RandomEvent[] subsequentEvent = new RandomEvent[1];
        }

        public string name;

        [TextArea] public string description;
        public EventChoice[] choices;
    }

    public RandomEvent[] events;

    // <========== Event Function Library ==========> //
    // Here you can code any function that would be used by an event that doesn't fit in any class in particular.

    public void ChangeGoldAmount(int amount)
    {
        CurrentRunInformations._goldAmount += amount;
    }
}
