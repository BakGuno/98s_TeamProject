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
  
  [Header("Ray")]
  private Camera _camera;
  
  private void Awake()
  {
    _camera = Camera.main;
    _animator = GetComponent<Animator>();
  }

  public override void OnAttackInput(Player conditions)
  {
      if (doesGatherResources || doesDealDamage)
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
          if (doesGatherResources && hit.collider.TryGetComponent(out GatherResource resource)) //TODO : Resource = 자원용 스크립트 얘기함.
          {
              resource.Gather(hit.point,hit.normal);
              //hit.collider.gameobject.name으로 해서 이게 어떤 아이템인지 판별할 수 있고, 여기서 다 처리할 수 있음.
              //오브젝트가 많아지면 대응이 어렵기때문에 각각의 오브젝트에 스크립트를 넣는다던지, 배열로 다 받아온다던지 하면 확장성도 떨어진다.
              //TODO : 여기서 프리팹 떨어지게는 할 수 있음. 근데 다 캐고나서 동작을 어떻게 해줘야될까. 용량이라던지 체크를해야되는데
          }

          if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damageable))
          {
              damageable.TakePhysicalDamage(damage);
          }
      }
  }
}
