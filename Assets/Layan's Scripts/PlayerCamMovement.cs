using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamMovement : MonoBehaviour
{
    [Header("Directions")]
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mousey = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation -= mousey;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
