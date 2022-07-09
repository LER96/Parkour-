using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft = 10;
    public TMP_Text timeUI;
    public bool over = false;

    private void Update()
    {
        Stoper();

    }

    void Stoper()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0 && timeLeft>60)
        {
            var minutes = Mathf.FloorToInt(timeLeft / 60);
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            if (seconds < 10)
            {
                timeUI.text = minutes + ":0" + seconds;
            }
            else
            {
                timeUI.text = minutes + ":" + seconds;
            }
            over = false;
        }
        else if(timeLeft<60 && timeLeft > 0)
        {
            Debug.Log("test");
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            timeUI.text = "" + seconds;
            over = false;
        }
        else if(timeLeft <= 0)
        {
            over = true;
            Debug.Log("over");
            timeLeft = 0;
            timeUI.text = "Times UP! Game OVER";
            //Time.timeScale = 0; 
        }
    }
}
