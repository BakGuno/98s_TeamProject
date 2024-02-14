using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [CanBeNull] private Player _player;
    public Transform handPosition;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();     
            if (_player != null)
            {
                _player.hasTorch = true;
                _player.takeRest = true;
            }
        }
    }


    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.takeRest = false;
            _player.hasTorch = false;
        }
        _player = null;
    }
}