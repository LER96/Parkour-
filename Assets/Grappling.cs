using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public Transform camera;
    public Transform player;
    public Transform point;
    public LayerMask grab;
    [SerializeField] float maxDistance=100;

    Vector3 grapplingPoint;
    [SerializeField] LineRenderer lrender;
    SpringJoint joint;

    private void Update()
    {
        DrawRopr();
        if(Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, grab))
        {
            grapplingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplingPoint;

            float distToPoint = Vector3.Distance(player.position, grapplingPoint);
            joint.maxDistance = distToPoint * 0.8f;
            joint.minDistance = distToPoint * 0.2f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }

    void StopGrapple()
    {

    }

    void DrawRopr()
    {
        lrender.SetPosition(0, point.position);
        lrender.SetPosition(1, grapplingPoint);

    }
}
