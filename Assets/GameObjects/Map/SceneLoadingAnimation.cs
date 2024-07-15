using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    string _sceneType;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAnimation(string sceneType)
    {
        _sceneType = sceneType;
        animator.SetTrigger("Fade In");
    }

    void LoadScene()
    {
        GI._roomType = _sceneType;
        GI._loader.LoadScene("Map", "Room");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
