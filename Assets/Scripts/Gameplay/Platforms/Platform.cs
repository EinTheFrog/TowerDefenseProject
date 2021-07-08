using UnityEngine;

namespace Gameplay.Platforms
{
    public abstract class Platform : MonoBehaviour
    {
        public Vector3 Center { get; private set; }

        private Vector3 _localPos;
        protected Vector3 Scale;
        protected Vector3 Size;

        public void OnDrawGizmos() // метод для упрощения постройки уровней в редакторе
        {
            var tform = transform;
            if (!tform.hasChanged) return;
            var localPosition = transform.localPosition;
            localPosition = new Vector3(
                Mathf.Round(localPosition.x),
                Mathf.Round(localPosition.y),
                Mathf.Round(localPosition.z)
            );
            tform.localPosition = localPosition;
            tform.hasChanged = false;
        }

        private void Awake() 
        {
            //выставляем Center удобный для нас
            _localPos = transform.localPosition;
            Scale = transform.localScale;
            Size = GetComponent<MeshFilter>().mesh.bounds.size;
            Center = new Vector3(
                _localPos.x + Scale.x * Size.x * 0.5f,
                Scale.y * Size.y,
                _localPos.z - Scale.z * Size.z * 0.5f
            );
        }
    }
}
