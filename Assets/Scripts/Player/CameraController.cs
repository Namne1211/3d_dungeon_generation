using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    Camera cam;

    float xRotation;
    float yRotation;

    private PlayerController _controller;
    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        //Lock Camera
        if(_controller.inventoryOn)
            return;
        
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}