using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MakeSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> sphere;
    private int index = 0;
    [SerializeField] private GameObject start;
    [SerializeField] private Text FPS;
    [SerializeField] private Text spheres;
    [SerializeField] private Light sunlight;

    int frameCount = 0;
    float dt = 0f;
    float fps = 0f;
    float updateRate = 4f;  // 4 updates per sec.


    void Start()
    {
        for (int i = 0; i < sphere.Count; i++)
        {
            float random = Random.Range(0.2f, 0.8f);
            sphere[i].transform.position = start.transform.position;
            sphere[i].SetActive(false);
            sphere[i].transform.localScale = new Vector3(random, random, random);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && index < sphere.Count)
        {
            sphere[index].SetActive(true);
            index++;
            spheres.text = index.ToString();
        }

        //FPS
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1f / updateRate;
        }

        FPS.text = fps.ToString("F1");

    }
    public void rotateLight(float newRotation)
    {
        Quaternion target = Quaternion.Euler(0, newRotation, 0);
        sunlight.transform.rotation = Quaternion.Slerp(sunlight.transform.rotation, target, Time.deltaTime * 2f);
    }
}
