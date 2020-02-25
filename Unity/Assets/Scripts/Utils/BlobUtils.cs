using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobUtils : MonoBehaviour
{
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = GetMouseWorldPosition(Input.mousePosition, Camera.main);
        pos.z = 0f;
        return pos;
    }

    public static Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera worldCamera)
    {
        screenPosition.z = worldCamera.transform.position.z;
        Vector3 pos = worldCamera.ScreenToWorldPoint(screenPosition);
        return pos;
    }
}
