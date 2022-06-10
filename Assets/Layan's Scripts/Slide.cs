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
    private float _slideTimer;
    public bool isSliding;

    public float slideYScale;
    private float _startYScale;

    [Header("Inputs")]
    private float _horizontalInput;
    private float _verticalInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = GetComponent<PlayerMovement>();
        _startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
        if (isSliding)
        {
            SlidingMovement();
        }
    }

    private void Inputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && _verticalInput != 0)
        {
            StartSlide();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && isSliding)
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (_movement.OnSlop() || _rb.velocity.y > -0.1f)
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
        isSliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);

    }
}
