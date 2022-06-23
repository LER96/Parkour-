using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [SerializeField] LineRenderer lineRender;
    [SerializeField] Transform hook;
    [SerializeField] Transform hookend;
    [SerializeField] Transform handPos;
    [SerializeField] Transform body;
    [SerializeField] LayerMask grapmask;
    [SerializeField] float maxGrapDist;
    [SerializeField] float dropDist;
    [SerializeField] float hookingSpeed;
    //[SerializeField] Vector3 offset;
    [SerializeField] Rigidbody rb;

    Vector3 hookpoint;
    bool isShooting;
    bool isGrappling;
    bool isSwingging;

    private void Start()
    {
        hook.position = handPos.position;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown((0)))
        {
            if (isGrappling || isShooting)
            {
                ResetAll();
            }
            else
            {
                ShootHook();
            }
        }
        if(isGrappling)
        {
            Grap();
        }
    }
    private void LateUpdate()
    {
        //draw the line
        if(lineRender.enabled)
        {
            lineRender.SetPosition(0, hookend.position);
            lineRender.SetPosition(1, handPos.position);
        }
    }

    //check if we hit the grappling point
    void ShootHook()
    {
       
        if (isShooting || isGrappling) return;

        isShooting = true;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, maxGrapDist, grapmask))
        {
            hookpoint = hit.point;
            isGrappling = true;
            hook.parent = null;
            hook.LookAt(hookpoint);
            print("Hit");
            lineRender.enabled = true;
        }

        isShooting = false;   
    }

    //if the hook touches the grapling Mask...
    //then set the body destenation to the hook point
    void Grap()
    {
        //offset = handPos.localPosition;
        hook.position = Vector3.Lerp(hook.position, hookpoint, hookingSpeed * Time.deltaTime);
        if (Vector3.Distance(hook.position, hookpoint) < 0.5f)
        {

            rb.isKinematic = true;
            body.position = Vector3.Lerp(body.position, hookpoint, hookingSpeed * Time.deltaTime);
            if (Vector3.Distance(body.position, hookpoint) < dropDist)
            {
                ResetAll();
            }
        }
    }

    void StartSwing()
    {

    }

    //after we the body reaches the target's location
    //the grappling stops
    void ResetAll()
    {
        isGrappling = false;
        rb.isKinematic = false;
        hook.SetParent(handPos);
        hook.localPosition = Vector3.zero;
        //hook.localRotation = Quaternion.EulerAngles(0, 0, 0);
        lineRender.enabled = false;
    }

}
