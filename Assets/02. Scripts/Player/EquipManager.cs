using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerMovements _controller;
    private Player _player;

    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        _controller = GetComponent<PlayerMovements>();
        _player = GetComponent<Player>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null&&_controller.canLook)
        {
            //TODO : 화면 중심점으로 캐릭터와 오브젝트 사이의 거리를 재서 충돌에 관한 처리를.
            _player._animator.SetTrigger("Attack");
            curEquip.OnAttackInput(_player);
        }
    }

    public void EquipNew(ItemData item)
    {
        UnEquip();
        curEquip =Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

}
