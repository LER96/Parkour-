using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamMovement : MonoBehaviour
{
    [Header("Directions")]
    public float sensitivity;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    [SerializeField] Texture2D target;

    //[SerializeField] CursorMode mode = CursorMode.Auto;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Cursor.SetCursor(target, Vector2.zero, mode);
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mousey = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        yRotation += mouseX;
        xRotation -= mousey;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
