using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterDamageHit : MonoBehaviour, IDamagable
{
    private SkinnedMeshRenderer _Renderer;
    protected MonsterController _monsterController;
    protected IDamagable damagable;

    private void Awake()
    {
        _Renderer = GetComponent<SkinnedMeshRenderer>();
        _monsterController = GetComponentInParent<MonsterController>();
        damagable = _monsterController.GetComponent<IDamagable>();
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
    public void TakePhysicalDamage(int damageAmount)
    {
        damagable.TakePhysicalDamage(damageAmount);
    }
    public void TakePhysicalBuff(int damageAmount)
    {

    }
}
