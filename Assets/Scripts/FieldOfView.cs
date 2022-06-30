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

    [SerializeField] float delay = 0.2f;

    public List<Transform> visibleTargets = new List<Transform>();


    private void Start()
    {
        StartCoroutine("FindTargets", delay);
    }

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
        Collider[] targetsInFieldView = Physics.OverlapSphere(transform.position, viewRadius, grapMask);
        for(int i=0; i< targetsInFieldView.Length; i++)
        {
            Transform target = targetsInFieldView[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
             Vector3 targetPos = Camera.main.WorldToViewportPoint(target.position);
            if (targetPos.z > 0 && targetPos.z < 1000 && targetPos.x > 0.35f && targetPos.x < 0.65f && targetPos.y > 0 && targetPos.y < 1)
            {
                float distTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, distTarget, obsMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegree, bool angelIsGlobal)
    {
        if(!angelIsGlobal)
        {
            angleInDegree += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));
    }
}
