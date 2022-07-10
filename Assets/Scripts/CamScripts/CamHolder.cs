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
        transform.position = camPosition.position;
    }
}
