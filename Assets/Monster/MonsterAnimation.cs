using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    Animator _animator;
    MonsterController _controller;

    private bool isDeath = false;
    public enum AttackType
    {
        RightAttack,
        LeftAttack,
        BiteAttack,
        StrongAttack,
        ButtAttack
    }
    AttackType attackType;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<MonsterController>();
    }
    public float StateInfo()
    {
        var animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return animStateInfo.length;
    }
    // State
    public void IdleAnimation()
    {
        var animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (animStateInfo.IsName("Idle") == false)
        {
            switch (_controller.monsterName)
            {
                case "Bear":
                    _animator.SetBool("Idle", true);
                    _animator.SetBool("WalkForward", false);
                    _animator.SetBool("RunForward", false);
                    break;
                case "Fox":
                    _animator.SetBool("Idle", true);
                    _animator.SetBool("WalkForward", false);
                    _animator.SetBool("RunForward", false);
                    break;
                case "Rabbit":
                    _animator.SetBool("Idle", true);
                    _animator.SetBool("WalkForward", false);
                    break;
            }
        }
    }
    public void RunAnimation()
    {
        switch (_controller.monsterName)
        {
            case "Bear":
                _animator.SetBool("Idle", false);
                _animator.SetBool("WalkForward", false);
                _animator.SetBool("RunForward", true);
                break;
            case "Fox":
                _animator.SetBool("Idle", false);
                _animator.SetBool("WalkForward", false);
                _animator.SetBool("RunForward", true);
                break;
            case "Rabbit":
                _animator.SetBool("Idle", false);
                _animator.SetBool("WalkForward", true);
                break;
        }
    }
    public void LookRotationAnimation()
    {
        _animator.SetBool("Idle", false);
        _animator.SetBool("WalkForward", true);
    }
    public void LookRotationEndAnimation()
    {
        _animator.SetBool("Idle", true);
        _animator.SetBool("WalkForward", false);
    }
    public void DeathAnimation()
    {
        if (isDeath == false)
        {
            isDeath = true;
            _animator.SetTrigger("Death");
        }
    }
    public void HitAnimation()
    {
        _animator.SetTrigger("Hit");
    }
    // Attck
    public AttackType AttackAnimation(string name)
    {
        float randomAttackType = UnityEngine.Random.Range(1f, 100f);
        switch (name)
        {
            case "Bear":

                if (randomAttackType <= 30 && randomAttackType > 0)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack1");
                    attackType = AttackType.RightAttack;
                }
                else if (randomAttackType <= 60 && randomAttackType > 30)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack2");
                    attackType = AttackType.LeftAttack;
                }
                else if (randomAttackType <= 80 && randomAttackType > 60)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack3");
                    attackType = AttackType.BiteAttack;
                }
                else if (randomAttackType <= 90 && randomAttackType > 80)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack5");
                    attackType = AttackType.StrongAttack;
                }
                else if (randomAttackType <= 100 && randomAttackType > 90)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Buff");
                    attackType = AttackType.ButtAttack;
                }
                break;
            case "Fox":
                if (randomAttackType <= 60 && randomAttackType > 0)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack1");
                    attackType = AttackType.RightAttack;
                }
                else if (randomAttackType <= 100 && randomAttackType > 60)
                {
                    _animator.SetBool("RunForward", false);
                    _animator.SetTrigger("Attack2");
                    attackType = AttackType.LeftAttack;
                }
                break;
        }
        return attackType;
    }
}
