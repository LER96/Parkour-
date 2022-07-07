using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private GameObject dialogue;
    private DialogueManager dialogueScript;

    const string ACTIVATION_TAG = "Player";


    private void Start()
    {
        dialogue = DialogueComponent.instance.dialogueManager;
        dialogueScript = dialogue.GetComponent<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(ACTIVATION_TAG) && dialogueScript.index != dialogueScript.tutorialPrompts.Length - 1)
        {
            dialogueScript.index++;
            gameObject.SetActive(false);
        }

        else
        {
            dialogueScript.dialogueBox.SetActive(false);
        }
    }
}
