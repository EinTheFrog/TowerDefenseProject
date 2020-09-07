using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    public Vector3 Center { get; private set; }

    void OnDrawGizmos() // метод для упрощения постройки уровней в редакторе
    {
        if (transform.hasChanged)
        {
            transform.localPosition = new Vector3(
                Mathf.Round(transform.localPosition.x),
                Mathf.Round(transform.localPosition.y),
                Mathf.Round(transform.localPosition.z)
                );
            transform.hasChanged = false;
        }
    }

    void Awake() 
    {
        //выставляем Center удобный для нас
        Vector3 localPos = transform.localPosition;
        Vector3 scale = transform.localScale;
        Vector3 size = GetComponent<MeshFilter>().mesh.bounds.size;
        Center = new Vector3(
            localPos.x + scale.x * size.x * 0.5f,
            scale.y * size.y,
            localPos.z - scale.z * size.z * 0.5f
            );
    }
}
