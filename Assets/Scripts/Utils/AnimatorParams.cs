using UnityEngine;

public static class AnimatorParams
{
    public static int IsMoving = Animator.StringToHash(nameof(IsMoving));
    public static int IsAlarmed = Animator.StringToHash(nameof(IsAlarmed));
}
