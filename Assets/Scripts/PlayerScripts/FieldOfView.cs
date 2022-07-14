using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float height;

    public LayerMask grapMask;
    public LayerMask obsMask;

    [SerializeField] float distanceToGrap = 100;
    [SerializeField] float delay = 0.2f;

    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine("FindTargets", delay);
    }

    //set an infinate loop that check each number of seconds if the player did catch something on sight
    IEnumerator FindTargets(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        //set an array of all the object, with the specific layer, that entered the cast sphere
        Collider[] targetsInFieldView = Physics.OverlapSphere(transform.position, viewRadius, grapMask);
        for(int i=0; i< targetsInFieldView.Length; i++)
        {
            Transform target = targetsInFieldView[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //gets the location of the target, that enerted the camera view
            Vector3 targetPos = Camera.main.WorldToViewportPoint(target.position);
            //if the position is on the middle of the camera view// that means the player is looking right at it
            if (targetPos.z > 0 && targetPos.z < distanceToGrap && targetPos.x > 0.35f && targetPos.x < 0.65f && targetPos.y > 0 && targetPos.y < 1)
            {
                float distTarget = Vector3.Distance(transform.position, target.position);
                //cast a ray that make sure that the target is not hiding behind anything
                if(!Physics.Raycast(transform.position, dirToTarget, distTarget, obsMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    //always make sure that the field of view is with the player rotation/position
    public Vector3 DirFromAngle(float angleInDegree, bool angelIsGlobal)
    {
        if(!angelIsGlobal)
        {
            angleInDegree += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));
    }
}
