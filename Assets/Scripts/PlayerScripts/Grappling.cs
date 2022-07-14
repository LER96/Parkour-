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
    [SerializeField] Rigidbody rb;
    Vector3 hookpoint;
    bool isShooting;
    bool isGrappling;


    FieldOfView fow;
    public bool ready;
    [SerializeField] GameObject redCross;
    Transform target;
    public List<Transform> grapOnSight = new List<Transform>();

    private void Start()
    {
        //sets the hook into the hand position
        hook.position = handPos.position;
    }
    private void Update()
    {

        CheckOnSight();
        LockOn();
        if (Input.GetMouseButtonDown((0)))
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
        //draw the rope from the handposition to the hook
        if(lineRender.enabled)
        {
            lineRender.SetPosition(0, hookend.position);
            lineRender.SetPosition(1, handPos.position);
        }
    }

    //takes the target lists from the field of view script
    void CheckOnSight()
    {
        fow = GetComponent<FieldOfView>();
        if (fow.visibleTargets.Count > 0)
        {
            //duplicate the list
            foreach (Transform onTarget in fow.visibleTargets)
            {
                if (grapOnSight.Count < fow.visibleTargets.Count)
                {
                    grapOnSight.Add(onTarget);
                }
            }
        }
        else
        {
            //delete the list
            grapOnSight.Clear();
        }
    }

    void LockOn()
    {
        //set the target icon onto the target position
        if(grapOnSight.Count>0)
        {
            redCross.SetActive(true);
            target = grapOnSight[0];
            redCross.transform.position = Camera.main.WorldToScreenPoint(target.position);
            ready = true;
        }
        else
        {
            redCross.SetActive(false);
            ready = false;
        }
    }

    //check if we hit the grappling point
    void ShootHook()
    {
        if (isShooting || isGrappling) return;

        isShooting = true;

        //if the player is looking at the target
        if (ready)
        {
            //hookpoint is the hook last position
            hookpoint = target.position;
            isGrappling = true;
            //hook becomes an object of his own// like a bullet
            hook.parent = null;
            hook.LookAt(hookpoint);
            print("Hit");
            //draw the line
            lineRender.enabled = true;
        }
        isShooting = false;
    }

    //if the hook touches the grapling Mask...
    //then set the body destenation to the hook point
    void Grap()
    {
        //create a smooth transition of the hook to the hookpoint 
        hook.position = Vector3.Lerp(hook.position, hookpoint, hookingSpeed * Time.deltaTime);

        //if the distance to the hook point is less than...// start the grapling movement
        if (Vector3.Distance(hook.position, hookpoint) < 0.9f)
        {
            rb.isKinematic = true;
            //player goes to the hookposition
            body.position = Vector3.Lerp(body.position, hookpoint, hookingSpeed * Time.deltaTime);
            if (Vector3.Distance(body.position, hookpoint) < dropDist)
            {
                ResetAll();
            }
        }
    }

    //after the body reaches the target's location
    //the grappling stops
    //hook becomes the hand child's again 
    void ResetAll()
    {
        isGrappling = false;
        rb.isKinematic = false;
        hook.SetParent(handPos);
        hook.localPosition = Vector3.zero;
        lineRender.enabled = false;
    }

}
