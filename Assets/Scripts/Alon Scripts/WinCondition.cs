using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    UIHandler loading;
    const string ACTIVATION_TAG = "Player";

    private void Start()
    {
        loading = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<UIHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == ACTIVATION_TAG)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                StartCoroutine(loading.LoadAsync(1));
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                StartCoroutine(loading.LoadAsync(0));
            }
        }
    }
}
