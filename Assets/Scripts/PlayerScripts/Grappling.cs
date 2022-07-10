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



    //Vector3 targetPos;
    FieldOfView fow;
    public bool ready;
    [SerializeField] GameObject redCross;
    Transform target;
    public List<Transform> grapOnSight = new List<Transform>();

    //List<GameObject> grapInGame = new List<GameObject>();


    private void Start()
    {
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
        //draw the line
        if(lineRender.enabled)
        {
            lineRender.SetPosition(0, hookend.position);
            lineRender.SetPosition(1, handPos.position);
        }
    }

    void CheckOnSight()
    {
        fow = GetComponent<FieldOfView>();
        if (fow.visibleTargets.Count > 0)
        {
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
            grapOnSight.Clear();
        }
        //    if(grapInGame.Count>0)
        //    {
        //        for(int i=0; i< grapInGame.Count; i++)
        //        {
        //            //takes the position of the object
        //            targetPos = Camera.main.WorldToViewportPoint(grapInGame[i].transform.localPosition);
        //            //if the object is in the range of sight (moves between 0-1)
        //            if (targetPos.z > 0 && targetPos.z<100 && targetPos.x > 0.35f && targetPos.x < 0.65f && targetPos.y > 0 && targetPos.y < 1)
        //            {
        //                //we cant have more grapling on sight then the game 
        //                if (grapOnSight.Count < grapInGame.Count)
        //                {
        //                    grapOnSight.Add(grapInGame[i]);
        //                }
        //            }
        //            //checks if it already 
        //            else if (grapOnSight.Contains(grapInGame[i]))
        //            {
        //                grapOnSight.Remove(grapInGame[i]); 
        //            }
        //        }
        //    }
        //    //Debug.Log("" + grapOnSight.Count);
    }



void LockOn()
    {
        //if (grapOnSight.Count > 0 && !locked && ready)
        //{
        //    locked = true;
        //    redCross.SetActive(true);
        //}
        //if (locked && grapOnSight.Count == 0)
        //{
        //    locked = false;
        //    redCross.SetActive(false);
        //    //ready = false;
        //}
        //if (locked && grapOnSight.Count > 0 && ready)
        //{
        //    target = grapOnSight[0].transform;
        //    redCross.transform.position = Camera.main.WorldToScreenPoint(target.position);
        //    //ready = true;
        //}
        if(grapOnSight.Count>0)
        {
            redCross.SetActive(true);
            target = grapOnSight[0];
            redCross.transform.position = Camera.main.WorldToScreenPoint(target.position);
            ready = true;
        }
        else
        {
            //locked = false;
            redCross.SetActive(false);
            ready = false;
        }
    }


    //check if we hit the grappling point
    void ShootHook()
    {

        if (isShooting || isGrappling) return;

        isShooting = true;
        if (ready)
        {
            hookpoint = target.position;
            isGrappling = true;
            hook.parent = null;
            hook.LookAt(hookpoint);
            print("Hit");
            lineRender.enabled = true;
        }
        isShooting = false;

        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit, maxGrapDist, grapmask))
        //{
        //    hookpoint = hit.point;
        //    isGrappling = true;
        //    hook.parent = null;
        //    hook.LookAt(hookpoint);
        //    print("Hit");
        //    lineRender.enabled = true;
        //}

        //isShooting = false;
    }

    //if the hook touches the grapling Mask...
    //then set the body destenation to the hook point
    void Grap()
    {
        hook.position = Vector3.Lerp(hook.position, hookpoint, hookingSpeed * Time.deltaTime);
        if (Vector3.Distance(hook.position, hookpoint) < 0.9f)
        {

            rb.isKinematic = true;
            body.position = Vector3.Lerp(body.position, hookpoint, hookingSpeed * Time.deltaTime);
            if (Vector3.Distance(body.position, hookpoint) < dropDist)
            {
                ResetAll();
            }
        }
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
