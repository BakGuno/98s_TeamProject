using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    [Range(0.0f, 1.0f)] 
    public float time;
    public float fullDayLength;
    public float startTime;
    private float timeRate;
   
    public Image clock;
    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update() //해가 떳다지는 한 순환 안에서 빛을 조절함.
    {
        time = (time - timeRate * Time.deltaTime) % 1.0f; 
        if (time <= 0)
        {
            Reset();
        }
        clock.transform.rotation = Quaternion.Euler(0, 0, time * 360f);

        if (time <= 0.25f)
        {
            GameManager.instance.daytime = _Time.Night;
        }
        // if (time <= 0.5)
        //     clock.color = Color.Lerp(clock.color, nightColor,  Time.deltaTime);
    }

    private void Reset()
    {
        time = startTime;
        GameManager.instance.daytime = _Time.Day;
    }
}
