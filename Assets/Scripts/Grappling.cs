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


    Vector3 targetPos;
    Transform target;
    [SerializeField] GameObject redCross;
    [SerializeField] bool locked;
    public bool ready;

    List<GameObject> grapInGame = new List<GameObject>();
    List<GameObject> grapOnSight = new List<GameObject>();


    private void Start()
    {
        hook.position = handPos.position;

        //collect all the grappling point in the game and add it into an list
        redCross.SetActive(false);
        GameObject[] allGrap = GameObject.FindGameObjectsWithTag("Grap");
        Debug.Log("there are" + allGrap.Length+ "Grapling ");
        foreach(GameObject a in allGrap)
        {
            grapInGame.Add(a);
        }
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
            else if(locked)
            {
                ShootHook();
            }
        }
        if(isGrappling)
        {
            Grap();
        }
    }

    void CheckOnSight()
    {
        if(grapInGame.Count>0)
        {
            for(int i=0; i< grapInGame.Count; i++)
            {
                //takes the position of the object
                targetPos = Camera.main.WorldToViewportPoint(grapInGame[i].transform.localPosition);
                //if the object is in the range of sight (moves between 0-1)
                if (targetPos.z > 0 && targetPos.z<100 && targetPos.x > 0.35f && targetPos.x < 0.65f && targetPos.y > 0 && targetPos.y < 1)
                {
                    //we cant have more grapling on sight then the game 
                    if (grapOnSight.Count < grapInGame.Count)
                    {
                        grapOnSight.Add(grapInGame[i]);
                    }
                }
                //checks if it already 
                else if (grapOnSight.Contains(grapInGame[i]))
                {
                    grapOnSight.Remove(grapInGame[i]); 
                }
            }
        }
        //Debug.Log("" + grapOnSight.Count);
    }

    void LockOn()
    {
        if (grapOnSight.Count > 0 && !locked)
        {
            locked = true;
            redCross.SetActive(true);
        }
        if (locked && grapOnSight.Count == 0)
        {
            locked = false;
            redCross.SetActive(false);
            ready = false;
        }
        if (locked && grapOnSight.Count > 0)
        {
            target = grapOnSight[0].transform;
            redCross.transform.position = Camera.main.WorldToScreenPoint(target.position);
            ready = true;
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
        //if(Physics.Raycast(ray, out hit, maxGrapDist, grapmask))
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
