using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide1 : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody _rb;
    private PlayerMovement1 _movement;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    [SerializeField] float _slideTimer;

    //public float slideYScale;
    //private float _startYScale;

    [Header("Inputs")]
    private float _horizontalInput;
    private float _verticalInput;

    [SerializeField] CapsuleCollider col;
    [SerializeField] CapsuleCollider slideCol;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = GetComponent<PlayerMovement1>();
        //_startYScale = playerObj.localScale.y;
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

        //if control is pressed while player is moving 
        if (Input.GetKeyDown(KeyCode.LeftControl) && (_verticalInput != 0 || _horizontalInput != 0))
        {
            StartSlide();
        }
        //checks if player stopped pressing control while already sliding 
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
        //playerObj.localScale = new Vector3(playerObj.localScale.x, playerObj.localScale.y, playerObj.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        //calculating input direction
        Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        //if you arent on slope, slide normally
        if (!_movement.OnSlop() || _rb.velocity.y > -0.1f)
        {
            //applying force for the slider
            _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        //if on slope, apply slop direction so player doesnt bounce
        else
        {
            _rb.AddForce(_movement.SlopDirection(inputDirection)* slideForce, ForceMode.Force);
        }

        //stops sliding when timer is at 0
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
        //playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
    }
}
