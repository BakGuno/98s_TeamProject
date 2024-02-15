using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeathEvent : MonoBehaviour
{
    private Player _player;
    public Camera mainCam;
    public Camera equipCam;
    public GameObject deathCam;
    //TODO : 애니메이션으로 변경.

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        _player.OnDieEvnet += DeathCameMove;
    }

    private void DeathCameMove()
    {
        if (_player.isDead)
        {
            mainCam.enabled = false;
            equipCam.enabled = false;
            deathCam.SetActive(true);
        }
    }
}