using UnityEngine;

public class FieldAnimation : MonoBehaviour
{
    private Animator _thisAnimator;
    private static readonly int TowerBuild = Animator.StringToHash("Build");
    private static readonly int FieldStop = Animator.StringToHash("Stop");
    private static readonly int FieldStart = Animator.StringToHash("Start");

    void OnEnable()
    {
        _thisAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation(Anim animType)
    {
        _thisAnimator.ResetTrigger(FieldStart);
        _thisAnimator.ResetTrigger(FieldStop);
        _thisAnimator.ResetTrigger(TowerBuild);
        if (animType == Anim.TowerBuild)
        {
            _thisAnimator.SetTrigger(TowerBuild);
        }

        if (animType == Anim.FieldStart)
        {
            _thisAnimator.SetTrigger(FieldStart);
        }
        if (animType == Anim.FieldStop)
        {
            _thisAnimator.SetTrigger(FieldStop);
        }
    }
    
    public enum Anim
    {
        FieldStart, FieldStop, TowerBuild
    }
}
