using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereLocationClamp : MonoBehaviour
{
    [SerializeField] GameObject start;
    void Update()
    {
        if (transform.position.y <= -15f)
        {
            //self.SetActive(false);
            transform.position = start.transform.position;
        }
    }
}
