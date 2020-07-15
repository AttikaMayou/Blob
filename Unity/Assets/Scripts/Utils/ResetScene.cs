using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class ResetScene : MonoBehaviour
    {
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
