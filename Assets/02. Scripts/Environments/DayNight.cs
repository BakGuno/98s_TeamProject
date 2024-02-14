using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    public Material dayTimeSkybox;
    public Material nightTimeSkybox;
    [Range(0.0f, 1.0f)] 
    public float time;
    public float fullDayLength;
    public float startTime;
    private float timeRate;
    public Vector3 noon;
    
    [Header("Sun")] 
    public Light sun;
    public Gradient sunColor; //그라데이션
    public AnimationCurve sunIntensity; //그래프에 맞춰 원하는 값들을 타임값을 꺼내올수있음

    [Header("Moon")] 
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")] 
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;
   
    public Image clock;
    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update() //해가 떳다지는 한 순환 안에서 빛을 조절함.
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;
        if (time < 0.75f) //TODO : 여길 조져야됨. 굳
        {
            SetTimeDay();
        }
        UpdateLighting(sun,sunColor,sunIntensity);
        UpdateLighting(moon,moonColor,moonIntensity);

        //환경광, 환경에서 받아오는 광들, 빛을 최소화시키려고함.
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time); 
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
        RenderSettings.skybox.SetFloat("_Rotation",time*360);
        clock.transform.rotation = Quaternion.Euler(0, 0, -time * 360f);
        if (time >= 0.75f)
        {
            SetTimeNight();
        }
        // if (time <= 0.5)
        //     clock.color = Color.Lerp(clock.color, nightColor,  Time.deltaTime);
    }
    
    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); //이거 초반에 본적 있음

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0 : 0.75f)) * noon * 4.0f; //noon은 90도
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity ==0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity >0 && !go.activeInHierarchy)
            go.SetActive(true);
    }

    private void SetTimeDay()
    {
        // ReSharper disable once RedundantCheckBeforeAssignment
        if(GameManager.instance.daytime != _Time.Day)
            GameManager.instance.daytime = _Time.Day;
        RenderSettings.skybox = dayTimeSkybox;
    }
    
    private void SetTimeNight()
    {
        // ReSharper disable once RedundantCheckBeforeAssignment
        if(GameManager.instance.daytime != _Time.Night)
            GameManager.instance.daytime = _Time.Night;
        RenderSettings.skybox = nightTimeSkybox;
    }
}
