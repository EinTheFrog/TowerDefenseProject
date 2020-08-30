using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MonoBehaviour
{

    Vector2 center;
    void OnDrawGizmos()
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

    void OnEnable()
    {
        Vector3 localPos = transform.localPosition;
        Vector3 scale = transform.localScale;
        Vector3 size = GetComponent<MeshFilter>().mesh.bounds.size;
        center = new Vector2(
            localPos.x + scale.x * size.x * 0.5f,
            localPos.z - scale.z * size.z * 0.5f
            );
    }

    public Vector3 GetCenterForHeight(float height)
    {
        return new Vector3(center.x, height, center.y);
    }

    public Vector3 GetCenterUnderPlatform()
    {
        return new Vector3(center.x, GetComponent<MeshRenderer>().bounds.size.y, center.y);
    }
}
