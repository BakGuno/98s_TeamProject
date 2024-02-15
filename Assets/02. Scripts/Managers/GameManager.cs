using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _Time{
    Day,
    Night
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public _Time daytime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
}
