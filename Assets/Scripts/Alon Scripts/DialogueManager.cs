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
        dialogueBox.SetActive(true);
        StartCoroutine(Type());
        index = _originalIndex;
    }

    private void Update()
    {
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

    IEnumerator Type()
    {
        foreach (char letter in tutorialPrompts[index].ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
