using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool :  Equip
{
  public float attackRate;
  private bool attacking;
  public float attackDistance;
  public float useStamina;

  [Header("Resource Gathering")]
  public bool doesGatherResources;

  [Header("Combat")]
  public bool doesDealDamage;

  public int damage;

  private Animator _animator;
  private Camera _camera;

  private void Awake()
  {
    _camera = Camera.main;
    _animator = GetComponent<Animator>();
  }

  public override void OnAttackInput(Player conditions)
  {
      if (!attacking)
      {
          if (conditions.UseStamina(useStamina)) //스태미나가 있을때만 공격할 수 있게
          {
              attacking = true;
              _animator.SetTrigger("Attack");
              Invoke("OnCanAttack", attackRate);
          }
      }
  }

  void OnCanAttack()
  {
      attacking = false;
  }

  public void OnHit()
  {
      Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2,0));
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit, attackDistance))
      {
          if (doesGatherResources && hit.collider.TryGetComponent( out Resource resource)) //TODO : Resource = 자원용 스크립트 얘기함.
          {
              resource.Gather(hit.point,hit.normal);
          }

          if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damageable))
          {
              damageable.TakePhysicalDamage(damage);
          }
      }
  }
}
