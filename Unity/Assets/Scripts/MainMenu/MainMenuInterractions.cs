using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInterractions : MonoBehaviour
{
    public void LoadECSScene()
    {
        SceneManager.LoadScene("ECS_MainScene");
    }

    public void LoadRaymarching1Scene()
    {
        SceneManager.LoadScene("Raymarching1");
    }

    public void LoadRaymarching2Scene()
    {
        SceneManager.LoadScene("Raymarching2");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
