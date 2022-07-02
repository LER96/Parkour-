using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    #region Singelton

    public static PlayerComponent instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;
}
