using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft = 10;
    public TMP_Text timeUI;
    public bool over;

    private void Update()
    {
        Stoper();

    }

    void Stoper()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            timeUI.text = ""+ seconds;
        }
        else
        {
            timeLeft = 0;
            timeUI.text = "Times UP! Game OVER";
            over = true;
            //Time.timeScale = 0; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Arrow")
        {
            var arrow = other.GetComponent<Coin>();
            timeLeft += arrow.value;
            other.gameObject.SetActive(false);
        }
    }
}
