using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    private Player _player;
    public float heat;
    //TODO : 횃불 등 다른 광, 열원도 추가해야함.
    private void Update()
    {
        if (_player != null)
        {
            
            _player.temperature.Heat(heat*Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();    
            if (!_player.hasTorch)
            {
                _player.takeRest = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!_player.hasTorch)
                _player.takeRest = false;
            _player = null;
        }
    }
    
    
}
