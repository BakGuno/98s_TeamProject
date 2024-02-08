using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    private Player _player;
    public float heat;
    
    private void Update()
    {
        if (_player != null)
        {
            _player.iswarm = true;
            _player.temperature.Heat(heat*Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.iswarm = false;
            _player = null;
        }
    }
    
    
}
