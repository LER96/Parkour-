using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    const string ACTIVATION_TAG = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(ACTIVATION_TAG))
        {
            //SceneManager.LoadScene(2);
            Debug.Log("Next Level");
        }
    }
}