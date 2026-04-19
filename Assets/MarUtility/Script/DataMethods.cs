using UnityEngine;

public class DataMethods
{
    #region Animation
    //Returns the animation clip with clipName if it exists in animator.
    public static AnimationClip FindAnimationClip(Animator animator, string target)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == target) return clip;
        return null;
    }
    #endregion
}
