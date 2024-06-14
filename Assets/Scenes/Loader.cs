using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class Loader : MonoBehaviour
{
    // Use the keys to act on .unity files. Make sure the associated values are up-to-date with the in-project file names !
    private Dictionary<string, string> _sceneEncylopedia = new Dictionary<string, string> {
        { "Map", "MapNavigation" },
        { "Room", "TestLvl" },
        { "MainMenu", "MenuScene" }};


    /* 
     EVENTS
    */
    // Event that takes both scene container's names concerned by the scene change
    public UnityEvent<string, string, LoadSceneMode> _UeSceneChange;


    /* 
     METHODS
    */
    public void Awake()
    {
        // Event subscribing
        _UeSceneChange.AddListener(LoadScene);

        // Ensure the object is always there
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        // Loads the first scene
        _UeSceneChange.Invoke("", _sceneEncylopedia["Map"], LoadSceneMode.Single);
    }


    // Scene loader
    public void LoadScene(string ARGcurScene, string ARGtargetScene, LoadSceneMode ARGloadMode)
    {
        // Check if the scene we are leaving is persistent (if it is, we need to do more work)
        if (true == GI.IsSceneContainerPersistent(ARGcurScene) && ARGloadMode == LoadSceneMode.Single)
        {
            // Check if the persistent scene was never "saved" before, and saves it if not
            if (true == GI.IsPersistentSceneContainerNull(ARGcurScene))
                GI.InstantiateAndCull(ARGcurScene);
            else
                GI._persistentSceneContainers[ (int) GI.SceneNametoEnum(ARGcurScene) ].SetActive(false);
        }

        // By this points, everything should be saved or culled, so let's change the scene :3
        SceneManager.LoadScene(ARGtargetScene, ARGloadMode);

        // This coroutine will Invoke events related to Load Scene, waiting for all Awake() of the loaded scene to resolve
        StartCoroutine(SceneLoadInvoker());
    }

    // Coroutine that waits the execution of every Awake() after a scene load to then raise events of a scene loading
    IEnumerator SceneLoadInvoker()
    {
        // Wait an amount of ticks before carrying on
        for (int i = 0;  i < 1;  ++i)
            yield return null;

        //   Raises scene load events
        // ! This MIGHT cause issues in the future, idrk
        foreach (UnityEvent UEvent in GI._SceneLoadUEventList)
            UEvent.Invoke();
    }
}
