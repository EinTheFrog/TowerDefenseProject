using UnityEngine;

public class FieldAnimation : MonoBehaviour
{
    private Animator thisAnimator;
    
    void Start()
    {
        thisAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation(Anim animType)
    {
        thisAnimator.ResetTrigger("Start");
        thisAnimator.ResetTrigger("Stop");
        switch (animType)
        {
            case Anim.FieldStart: thisAnimator.SetTrigger("Start"); break;
            case Anim.FieldStop: thisAnimator.SetTrigger("Stop"); break;
        }
    }
    
    public enum Anim
    {
        FieldStart, FieldStop
    }
}
