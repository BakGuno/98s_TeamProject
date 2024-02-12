using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [CanBeNull] private Player _player;
    public Transform handPosition;

    // private void OnTriggerEnter(Collider other)
    // {
    //     
    //     _player = other.GetComponent<Player>();
    //     if (_player != null)
    //     {
    //         _player.takeRest = true;
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         _player = other.GetComponent<Player>();     
    //         if (_player != null)
    //         {
    //             _player.takeRest = true;
    //         }
    //     }
    // }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();     
            if (_player != null)
            {
                _player.takeRest = true;
            }
        }
    }
    
  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.takeRest = true;
            }
            _player = null;
        }
    }

    private void Update()
    {
        gameObject.transform.position = new Vector3(handPosition.transform.position.x + 0.5f,
            handPosition.transform.position.y, handPosition.transform.position.z + 0.9f);
    }
}
