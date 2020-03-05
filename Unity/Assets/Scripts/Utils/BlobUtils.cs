using UnityEngine;

//Author : Attika

namespace Utils
{
    public class BlobUtils : MonoBehaviour
    {
        public static Vector3 GetMouseWorldPosition()
        {
            var pos = GetMouseWorldPosition(Input.mousePosition, Camera.main);
            return pos;
        }

        public static Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera worldCamera)
        {
            //screenPosition.z = worldCamera.transform.position.z;
            var pos = worldCamera.ScreenToWorldPoint(screenPosition);
            return pos;
        }
    }
}
