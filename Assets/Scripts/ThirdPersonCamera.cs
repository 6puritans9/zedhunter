using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Transform cameraRig; // Reference to the camera rig (pivot point)
    public float distance = 5f; // Distance from the player
    public float height = 2f; // Height above the player
    public float rotationSpeed = 5f; // Speed of camera rotation
    public float smoothSpeed = 0.1f; // Speed of smoothing

    private float mouseX, mouseY;
    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Get mouse input
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60); // Clamp vertical rotation

        // Rotate the camera rig based on mouse input
        cameraRig.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        // Rotate the player to face the same direction as the camera (only on the Y-axis)
        Vector3 playerDirection = new Vector3(cameraRig.forward.x, 0, cameraRig.forward.z);
        player.rotation = Quaternion.Slerp(player.rotation, Quaternion.LookRotation(playerDirection), smoothSpeed);

        // Calculate desired position for the camera
        Vector3 desiredPosition = cameraRig.position + cameraRig.rotation * new Vector3(0, height, -distance);

        // Smooth the camera position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);

        // Look at the player
        transform.LookAt(cameraRig.position);
    }
}