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
    private Color dayColor = new Color(253/255f,249/255f,32/255f);
    private Color nightColor = new Color(153f/255f,178f/255f,204f/255f);

    public Image clockBG;
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

        if (time <= 0.5)
            clock.color = Color.Lerp(clock.color, nightColor,  Time.deltaTime);
            
        
            
        clockBG.fillAmount = time;
    }

    private void Reset()
    {
        time = startTime;
        clock.color = dayColor;
    }
}
