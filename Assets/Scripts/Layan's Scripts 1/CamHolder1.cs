using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHolder1 : MonoBehaviour
{
    public Transform camPosition;

    private void Update()
    {
        transform.position = camPosition.position;
    }
}
