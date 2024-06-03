using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [SerializeField] GameObject _map;

    void Start()
    {
        if(GlobalInformations._map == null)
        {
            GlobalInformations._map = Instantiate(_map);
            DontDestroyOnLoad(GlobalInformations._map);
        }
        else
        {
            GlobalInformations._map.SetActive(true);
        }
    }
}
