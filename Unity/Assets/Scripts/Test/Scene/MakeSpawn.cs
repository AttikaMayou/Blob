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
        if (Input.GetKeyDown(KeyCode.Return) && index < sphere.Count)
        {
            sphere[index].SetActive(true);
            index++;
            spheres.text = "Number of spheres : " + index.ToString();
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

        FPS.text = "FPS : " + fps.ToString("F1");

    }
}
