using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimatorHelper
{
    public static AnimationClip GetAnimationClip(Animator animator, string id)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == id)
                return clip;
        }
        return null;
    }
    public static float GetAnimationLength(Animator animator, string id)
    {
        AnimationClip clip = GetAnimationClip(animator, id);
        if (clip)
            return clip.length;
        return 0;
    }

    public static float GetAnimationCurrentTime(Animator animator, int layer = 0)
    {
        AnimationClip clip = GetAnimationClip(animator, GetCurrentAnimationName(animator, layer));
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(layer);

        return clip.length * animState.normalizedTime;
    }

    // https://discussions.unity.com/t/getting-the-current-frame-of-an-animation-clip/610627
    public static int GetAnimationCurrentFrame(Animator animator, int layer = 0)
    {
        AnimatorClipInfo clip = animator.GetCurrentAnimatorClipInfo(layer)[0];

        return (int)(GetAnimationCurrentTime(animator, layer)*clip.clip.frameRate);
    }

    public static string GetCurrentAnimationName(Animator animator, int layer = 0)
    {
        foreach (var clip in GetArrayAnimationClips(animator))
        {
            if (animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(clip.name))
                return clip.name;
        }
        return null;
    }

    public static AnimationClip[] GetArrayAnimationClips(Animator animator)
    {
        return animator.runtimeAnimatorController.animationClips;
    }
    public static List<AnimationClip> GetListAnimationClips(Animator animator)
    {
        return animator.runtimeAnimatorController.animationClips.ToList();
    }
}
