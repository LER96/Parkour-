using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody _rb;
    private PlayerMovement _movement;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    [SerializeField] float _slideTimer;

    [Header("Inputs")]
    private float _horizontalInput;
    private float _verticalInput;

    [SerializeField] CapsuleCollider col;
    [SerializeField] CapsuleCollider slideCol;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
        if (_movement.sliding)
        {
            SlidingMovement();
        }
    }

    private void Inputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && (_verticalInput != 0 || _horizontalInput != 0))
        {
            StartSlide();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && _movement.sliding)
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        col.enabled = false;
        slideCol.enabled = true;
        _movement.sliding = true;
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        //calculating input direction
        Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (!_movement.OnSlop() || _rb.velocity.y > -0.1f)
        {
            _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        else
        {
            _rb.AddForce(_movement.SlopDirection(inputDirection)* slideForce, ForceMode.Force);
        }

        if (_slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        col.enabled = true;
        slideCol.enabled = false;
        _movement.sliding = false;
    }
}
