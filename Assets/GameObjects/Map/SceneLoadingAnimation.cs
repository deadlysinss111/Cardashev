using System.Collections;
using Unity.VisualScripting;
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

        animator.SetTrigger("Fade Out");
    }

    public void StartAnimation(string sceneType, bool altLoad=false)
    {
        animator.SetTrigger("Fade In");
        if (altLoad == false)
        {
            roomIcon1.sprite = GI._currentRoomIcon;
            roomIcon2.sprite = GI._currentRoomIcon;
            _sceneType = sceneType;
        }
        else
        {
            StartCoroutine(AltLoadWaitFade(sceneType)); // One hell of a bandage
        }
    }

    IEnumerator AltLoadWaitFade(string nextMap)
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade In") == false)
        {
            yield return null;
        }
        yield return new WaitForNextFrameUnit(); // Wait a frame for the animation to start
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade In") && AnimatorHelper.GetAnimationCurrentTime(animator) <= AnimatorHelper.GetAnimationLength(animator, "Fade In"))
        {
            yield return null;
        }
        GI._loader.LoadScene("MainMenu", nextMap); // One hell of a bandage
    }

    void LoadScene()
    {
        GI._roomType = _sceneType;
        GI._loader.LoadRoom("Map");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
