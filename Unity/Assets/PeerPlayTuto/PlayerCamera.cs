using UnityEngine;
using System.Collections;

public class PlayerCamera: MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float sensitivity = 1;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
        if(speed < 0)
        {
            speed = 0;
        }

        if (Input.GetMouseButton(1))
        {
            yaw += sensitivity * Input.GetAxis("Mouse X");
            pitch -= sensitivity * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            Camera.main.transform.Translate(Vector3.forward * Input.GetAxisRaw("Vertical") * speed);
        }
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            Camera.main.transform.Translate(Vector3.right * Input.GetAxisRaw("Horizontal") * speed);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            speed += 0.1f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            speed -= 0.1f;
        }
    }
}


  