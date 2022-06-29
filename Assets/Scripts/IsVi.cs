using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsVi : MonoBehaviour
{
    public bool sight;
    private void OnBecameVisible()
    {
        sight = true;
    }
    private void OnBecameInvisible()
    {
        sight = false;
    }
}
