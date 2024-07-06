using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 2.0f;
    public Transform playerCamera;
    
    private float pitch = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        float moveForward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        Vector3 move = transform.right * moveSide + transform.forward * moveForward;
        transform.position += move;
    }
}
