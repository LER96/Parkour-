using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : MonoBehaviour
{
    #region Singelton

    public static DialogueComponent instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject dialogueManager;
}
