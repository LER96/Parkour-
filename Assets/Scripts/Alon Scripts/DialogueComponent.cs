using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : MonoBehaviour
{
   // Usage of singelton to assign all objects of the prefab this component at start
    #region Singelton

    public static DialogueComponent instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject dialogueManager;
}
