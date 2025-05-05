using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;               // Transform arriba del personaje
    public float baseDistance = 5f;        // Distancia base en Z
    public float maxDistance = 8f;         // Distancia al correr (shift)
    public float mouseSensitivity = 3f;

    public float pitchMin = -40f;
    public float pitchMax = 60f;

    private float yaw = 0f;
    private float pitch = 20f;

    private float currentDistance; // Distancia actual Z
    private bool isShiftReleased = false;
    private float releaseTime = 0f;

    [Header("Lerp Settings")]
    public float lerpSpeed = 2f;       // Velocidad al alejar (Shift presionado)
    public float lerpSpeedStop = 1f;   // Velocidad al dejar de moverse
    public float shiftReleaseDelay = 1f;       // Delay antes de acercar al soltar Shift

    [Header("Pitch Influence Settings")]
    public float yOffsetMin = -1f;     // Cuánto baja la cámara al mirar abajo
    public float yOffsetMax = 1f;      // Cuánto sube al mirar arriba
    public float zOffsetMin = -1f;     // Se aleja (Z negativo) al mirar abajo
    public float zOffsetMax = 1f;      // Se acerca (Z positivo) al mirar arriba

    void Start()
    {
        currentDistance = baseDistance;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        bool isMoving = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).sqrMagnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        if (!isRunning && Input.GetKeyUp(KeyCode.LeftShift))
        {
            isShiftReleased = true;
            releaseTime = Time.time;
        }

        if (isRunning)
        {
            currentDistance = Mathf.Lerp(currentDistance, maxDistance, Time.deltaTime * lerpSpeed);
            isShiftReleased = false;
        }
        else if (!isRunning && isShiftReleased && Time.time > releaseTime + shiftReleaseDelay)
        {
            currentDistance = Mathf.Lerp(currentDistance, baseDistance, Time.deltaTime * lerpSpeedStop);
        }

        // Factor de inclinación (0 = mirando abajo, 1 = mirando arriba)
        float verticalFactor = Mathf.InverseLerp(pitchMin, pitchMax, pitch);

        // Offset vertical en Y (más alto al mirar arriba, más bajo al mirar abajo)
        float yOffset = Mathf.Lerp(yOffsetMin, yOffsetMax, verticalFactor);
        // Offset en Z (más cerca al mirar arriba, más lejos al mirar abajo)
        float zOffset = Mathf.Lerp(zOffsetMin, zOffsetMax, verticalFactor);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Posición final teniendo en cuenta rotación y offset
        Vector3 cameraOffset = rotation * new Vector3(0f, yOffset, -(currentDistance + zOffset));
        transform.position = target.position + cameraOffset;

        transform.LookAt(target.position);
    }
}
