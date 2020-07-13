using UnityEngine;

namespace Utils
{
    public class InstructionsPopUp : MonoBehaviour
    {
        public GameObject instructions;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                PopUp();
            }
        }

        private void PopUp()
        {
            instructions.gameObject.SetActive(!instructions.activeSelf);
        }
    }
}
