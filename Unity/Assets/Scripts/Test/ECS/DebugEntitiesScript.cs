using System.Collections;
using UnityEngine;

//Author : Attika

namespace Test
{
    public class DebugEntitiesScript : MonoBehaviour
    {
        public float timeBeforeDestroy = 5.0f;
        
        private void Awake()
        {
            StartCoroutine(WaitForDestroy());
        }

        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(timeBeforeDestroy);
            Destroy(this.gameObject);
        }
    }
}
