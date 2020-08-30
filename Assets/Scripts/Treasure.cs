using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    float deceleration = 0;
    [SerializeField]
    float levitateHeight = 0;
    public float Deceleration { get { return deceleration; } }

    public bool isCaptured { get; set; }

    public Vector3 PositionForEnemies
    {
        get
        {
            return new Vector3(
                transform.localPosition.x,
                transform.localPosition.y - (transform.localScale.y * 0.5f + levitateHeight),
                transform.localPosition.z);
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.localPosition = newPosition;
        transform.localPosition += Vector3.up * (transform.localScale.y * 0.5f + levitateHeight);
    }
    
}
