using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHolder : MonoBehaviour
{
    public Transform camPosition;
    [SerializeField]  PlayerMovement1 wR;
    [SerializeField] Transform originalPose;

    private void Update()
    {
        if (wR.wallRunning)
        {
            
            transform.SetParent(camPosition);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.parent = null;
            transform.position = camPosition.position;
        }
    }
}
