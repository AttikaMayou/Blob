using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightOrientation : MonoBehaviour
{
    [SerializeField] Transform light;
    [SerializeField] Slider slider;
    private float angle;
    private Vector3 pos;

    // Update is called once per frame
    void Update()
    {
        pos = light.position + new Vector3(Mathf.Cos(angle), -2f, Mathf.Sin(angle)) * Time.deltaTime;
        light.transform.LookAt(pos);
    }

    public void SetAngle()
    {
        angle = slider.value * Mathf.Deg2Rad;
    }
}
