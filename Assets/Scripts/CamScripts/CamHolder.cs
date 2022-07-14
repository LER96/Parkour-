using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHolder : MonoBehaviour
{
    public Transform camPosition;
   
    private void Update()
    {
        transform.position = camPosition.position;
    }
}
