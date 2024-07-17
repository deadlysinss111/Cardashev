using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Image roomIcon1;
    [SerializeField] Image roomIcon2;
    string _sceneType;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(StartFadeOut());
    }

    IEnumerator StartFadeOut()
    {
        roomIcon1.sprite = GI._currentRoomIcon;
        roomIcon2.sprite = GI._currentRoomIcon;

        yield return new WaitForSecondsRealtime(.5f);

        print("before" + roomIcon1.sprite);

        animator.SetTrigger("Fade Out");

        print("after" + roomIcon1.sprite);
    }

    public void StartAnimation(string sceneType)
    {
        roomIcon1.sprite = GI._currentRoomIcon;
        roomIcon2.sprite = GI._currentRoomIcon;
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
