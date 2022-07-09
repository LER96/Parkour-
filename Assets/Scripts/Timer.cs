using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

[System.Serializable]
public  class Timer : MonoBehaviour
{
    public static float time;

    static Timer()
    {
        time = 90;
    }



    //public float timeLeft = 10;
    //public TMP_Text time;
    //public bool over = false;

    //private void Update()
    //{
    //    Stoper();

    //}

    //void Stoper()
    //{
    //    timeLeft -= Time.deltaTime;
    //    if (timeLeft > 0 && timeLeft>60)
    //    {
    //        var minutes = Mathf.FloorToInt(timeLeft / 60);
    //        var seconds = Mathf.FloorToInt(timeLeft % 60);
    //        if (seconds < 10)
    //        {
    //            time.text = minutes + ":0" + seconds;
    //        }
    //        else
    //        {
    //            time.text = minutes + ":" + seconds;
    //        }
    //        over = false;
    //    }
    //    else if(timeLeft<60 && timeLeft > 0)
    //    {
    //        Debug.Log("test");
    //        var seconds = Mathf.FloorToInt(timeLeft % 60);
    //        time.text = "" + seconds;
    //        over = false;
    //    }
    //    else if(timeLeft <= 0)
    //    {
    //        over = true;
    //        Debug.Log("over");
    //        timeLeft = 0;
    //        time.text = "Times UP! Game OVER";
    //        //Time.timeScale = 0; 
    //    }
    //}
}

