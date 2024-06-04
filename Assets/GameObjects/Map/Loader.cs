using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    /* 
     FIELDS
    */
    // Nothing yet


    /* 
     PROPERTIES
    */
    // Nothing yet


    /* 
     EVENTS
    */
    // Event that takes both scene container's names concerned by the scene change
    public UnityEvent<string, string, LoadSceneMode> _UeSceneChange;


    /* 
     METHODS
    */
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
                GI._persistentSceneContainers[(int)GI.SceneNametoEnum(ARGcurScene)].SetActive(false);
        }

        // By this points, everything should be saved or culled, so let's change the scene :3
        SceneManager.LoadScene(ARGtargetScene, ARGloadMode);
    }

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
        _UeSceneChange.Invoke("", "Map", LoadSceneMode.Single);
    }



    // ~~~~~~
    // Old code to load the map only
    /*
    [SerializeField] GameObject _map;

    void Start()
    {
        if (GlobalInformations._map == null)
        {
            GlobalInformations._map = Instantiate(_map);
            DontDestroyOnLoad(GlobalInformations._map);
        }
        else
        {
            GlobalInformations._map.SetActive(true);
        }
    }

    // Old code to load the map only
    // ~~~~~~
    */
}
