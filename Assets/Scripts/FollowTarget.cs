using UnityEngine;

public class FollowTarget : MonoBehaviour {
    [Header("Camera Tracking")]
    [SerializeField] private Transform followTarget;
   
    [SerializeField] private float rotationalSpeed = 10f;
    [SerializeField] private float bottomClamp = -40f;
    [SerializeField] private float topClamp = 70f;

    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    private void LateUpdate() {
        CameraLogic();
    }

    private void CameraLogic() {
        float mouseX = GEtMouseInput("Mouse X");
        float mouseY = GEtMouseInput("Mouse Y");

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, bottomClamp, topClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);
    }
   
    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis) {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }

    private float GEtMouseInput(string axis) {
        return Input.GetAxis(axis) * rotationalSpeed * Time.deltaTime;
    }
}