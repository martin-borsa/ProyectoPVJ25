using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;
    public float baseDistance = 5f;
    public float maxDistance = 8f;
    public float mouseSensitivity = 3f;
    public float pitchMin = -40f;
    public float pitchMax = 60f;

    [Header("Run Offset Settings")]
    public float distanceLerpSpeed = 2f;
    public float distanceLerpSpeedStop = 1f;
    public float shiftReleaseDelay = 1f;

    [Header("Pitch Influence Settings")]
    public float yOffsetMin = -1f;
    public float yOffsetMax = 1f;
    public float zOffsetMin = -1f;
    public float zOffsetMax = 1f;

    [Header("Aiming Settings")]
    public float aimXOffset = 1.5f;
    public float aimYOffset = 0.5f;
    public float aimZOffset = 1f;
    public float offsetLerpSpeed = 5f;
    public float aimFOV = 40f;
    public float normalFOV = 60f;
    public float fovLerpSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 20f;
    private float currentDistance;
    private bool isShiftReleased = false;
    private float releaseTime = 0f;

    private float currentXVisualOffset = 0f;
    private float currentXCamOffset = 0f;
    private float currentYCamOffset = 0f;
    private float currentZCamOffset = 0f;
    private Camera cam;

    void Start()
    {
        currentDistance = baseDistance;
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        cam.fieldOfView = normalFOV;
    }

    void LateUpdate()
    {
        // Rotación con mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Movimiento y correr
        bool isMoving = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).sqrMagnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;
        if (!isRunning && Input.GetKeyUp(KeyCode.LeftShift))
        {
            isShiftReleased = true;
            releaseTime = Time.time;
        }

        if (isRunning)
        {
            currentDistance = Mathf.Lerp(currentDistance, maxDistance, Time.deltaTime * distanceLerpSpeed);
            isShiftReleased = false;
        }
        else if (isShiftReleased && Time.time > releaseTime + shiftReleaseDelay)
        {
            currentDistance = Mathf.Lerp(currentDistance, baseDistance, Time.deltaTime * distanceLerpSpeedStop);
        }

        // Pitch offsets
        float verticalFactor = Mathf.InverseLerp(pitchMin, pitchMax, pitch);
        float yOffset = Mathf.Lerp(yOffsetMin, yOffsetMax, verticalFactor);
        float zOffset = Mathf.Lerp(zOffsetMin, zOffsetMax, verticalFactor);

        // Aiming (click derecho)
        bool isAiming = Input.GetMouseButton(1);

        float targetXVisualOffset = isAiming ? aimXOffset : 0f;
        float targetXCamOffset = isAiming ? aimXOffset : 0f;
        float targetYCamOffset = isAiming ? aimYOffset : 0f;
        float targetZCamOffset = isAiming ? aimZOffset : 0f;

        // Lerp para desplazamientos de cámara
        currentXVisualOffset = Mathf.Lerp(currentXVisualOffset, targetXVisualOffset, Time.deltaTime * offsetLerpSpeed);
        currentXCamOffset = Mathf.Lerp(currentXCamOffset, targetXCamOffset, Time.deltaTime * offsetLerpSpeed);
        currentYCamOffset = Mathf.Lerp(currentYCamOffset, targetYCamOffset, Time.deltaTime * offsetLerpSpeed);
        currentZCamOffset = Mathf.Lerp(currentZCamOffset, targetZCamOffset, Time.deltaTime * offsetLerpSpeed);

        // FOV
        float targetFOV = isAiming ? aimFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);

        // Calcular la posición de la cámara
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = new Vector3(currentXCamOffset, yOffset + currentYCamOffset, -(currentDistance + zOffset - currentZCamOffset));
        Vector3 cameraOffset = rotation * offset;
        transform.position = target.position + cameraOffset;

        // Punto de enfoque desplazado en X
        Vector3 visualTargetPos = target.position + target.right * currentXVisualOffset;
        transform.LookAt(visualTargetPos);
    }
}