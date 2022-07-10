using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public string[] tutorialPrompts;
    public GameObject dialogueBox;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float textSpeed;

    public int index;
    private int _originalIndex;

    private void Start()
    {
        // Starts the tutorial prompts
        dialogueBox.SetActive(true);
        StartCoroutine(Type());
        index = _originalIndex;
    }

    private void Update()
    {
        // Checks if the player reached a tutorial waypoint, if so, updates prompt
        if (index != _originalIndex)
        {
            text.text = "";
            StartCoroutine(Type());
            _originalIndex = index;
        }

        else if (index != tutorialPrompts.Length && index != _originalIndex)
        {
            dialogueBox.SetActive(false);
        }
    }

    // Creates the appearing letters effect
    IEnumerator Type()
    {
        foreach (char letter in tutorialPrompts[index].ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
