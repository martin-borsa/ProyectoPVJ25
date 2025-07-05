using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Movimiento")] //Rotacion 1)
    [SerializeField] private float rotationSpeed = 180;
    [SerializeField] private float floatHeight = 0.4f;
    [SerializeField] private float floatSpeed = 1.8f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float ramdomOffSet;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private Space rotationSpace = Space.World;

    private void Start() //Rotacion 2)
    {
        startPosition = transform.position;
        ramdomOffSet = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        RotateObjecto();
        //FloatObject
    }

    #region MOVIMIENTO DE OBJETO
    private void RotateObjecto()
    {
        transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, rotationSpace);
    }

    private void FloatObject()
    {
        float targetY = startPosition.y + Mathf.Sin(Time.deltaTime * floatSpeed + ramdomOffSet) * floatHeight;
        transform.position = new Vector3(startPosition.x, targetY, startPosition.z);
    }
    #endregion

}
