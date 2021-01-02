using UnityEngine;

namespace Gameplay.Towers
{
    public class FieldAnimation : MonoBehaviour
    {
        private Animator _thisAnimator;
        private readonly int _towerBuild = Animator.StringToHash("Build");
        private readonly int _fieldStop = Animator.StringToHash("Stop");
        private readonly int _fieldStart = Animator.StringToHash("Start");

        void OnEnable()
        {
            _thisAnimator = GetComponent<Animator>();
        }

        public void PlayAnimation(Anim animType)
        {
            _thisAnimator.ResetTrigger(_fieldStart);
            _thisAnimator.ResetTrigger(_fieldStop);
            _thisAnimator.ResetTrigger(_towerBuild);
            if (animType == Anim.TowerBuild)
            {
                _thisAnimator.SetTrigger(_towerBuild);
            }

            if (animType == Anim.FieldStart)
            {
                _thisAnimator.SetTrigger(_fieldStart);
            }
            if (animType == Anim.FieldStop)
            {
                _thisAnimator.SetTrigger(_fieldStop);
            }
        }
    
        public enum Anim
        {
            FieldStart, FieldStop, TowerBuild
        }
    }
}
