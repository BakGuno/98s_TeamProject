using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterDamageHit : MonoBehaviour
{
    private SkinnedMeshRenderer _Renderer;

    private void Awake()
    {
        _Renderer = GetComponent<SkinnedMeshRenderer>();
    }
    public void Hit()
    {
        StartCoroutine(HitRenderer());
    }
    private IEnumerator HitRenderer()
    {
        _Renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _Renderer.material.color = Color.white;
        yield break;
    }
}
