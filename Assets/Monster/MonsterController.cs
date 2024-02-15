using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static MonsterAnimation;
using static UnityEditor.FilePathAttribute;

public class MonsterController : MonoBehaviour
{
    [Header("Nav")]
    [SerializeField] protected Transform target;
    [SerializeField] protected NavMeshAgent nvAgent;

    [Header("Info")]
    [SerializeField] protected UnitAbility unitAbility;

    protected MonsterAnimation _MonsterAnimator;
    protected Rigidbody _rigidbody;
    protected CapsuleCollider _collider;
    protected MonsterController _monsterController;
    protected MonsterDamageHit _monsterDamageHit;

    [HideInInspector]
    public string monsterName;
    protected float hp;
    protected float ad;
    protected float df;
    protected float moveSpeed;
    protected float adSpeed;
    protected MonsterType monsterType;

    protected RaycastHit _hit;
    protected float rayRange;
    protected float rayExtent;

    protected float updateInterval = 3f;
    protected bool friendlyMove = true;

    public enum State
    {
        IDLE,
        RUN,
        ATTACK,
        DEATH
    }
    State state = State.IDLE;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _MonsterAnimator = GetComponent<MonsterAnimation>();
        _rigidbody = GetComponent<Rigidbody>();
        nvAgent = GetComponent<NavMeshAgent>();
        _monsterDamageHit = transform.Find("Body").GetComponent<MonsterDamageHit>();
    }
    private void Start()
    {
        //Monster Info
        monsterName = unitAbility.name;
        hp = unitAbility.hp;
        ad = unitAbility.offensePower;
        df = unitAbility.defensePower;
        adSpeed = unitAbility.attackSpeed;
        monsterType = unitAbility.type;
        _collider.radius = unitAbility.detectionRange;
        rayRange = unitAbility.attackRange;
        rayExtent = unitAbility.ExtentRange;

        nvAgent.speed = unitAbility.moveSpeed;

        StartCoroutine(StateMachine());
    }
    private IEnumerator StateMachine()
    {
        while (hp > 0)
        {
            yield return StartCoroutine(state.ToString());
        }
        StartCoroutine(DEATH());
    }
    private IEnumerator IDLE()
    {
        _MonsterAnimator.IdleAnimation();

        int dir = UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        float lookSpeed = UnityEngine.Random.Range(25f, 100f);
        float lookRotation = UnityEngine.Random.Range(4f, 10f);
        float idleMoveSpeed = UnityEngine.Random.Range(0.5f, 3f);

        yield return new WaitForSeconds(lookRotation);
        _MonsterAnimator.LookRotationAnimation();

        for (float i = 0; i < _MonsterAnimator.StateInfo(); i += Time.deltaTime)
        {
            _rigidbody.velocity = Vector3.zero;
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y + (dir) * Time.deltaTime * lookSpeed, 0f);
            var forward = Vector3.forward;
            _rigidbody.velocity = transform.TransformDirection(forward) * idleMoveSpeed;
            yield return null;
        }
        _MonsterAnimator.LookRotationEndAnimation();
        _rigidbody.velocity = Vector3.zero;
    }
    private IEnumerator RUN()
    {
        yield return null;
        if (nvAgent.isStopped)
        {
            nvAgent.isStopped = false;
        }
        _MonsterAnimator.RunAnimation();
        if (monsterType == MonsterType.friendly)
        {
            while (friendlyMove)
            {
                Vector3 randomPosition = GetRandomPosition();
                yield return new WaitForSeconds(updateInterval);
                nvAgent.SetDestination(randomPosition);
            }
            if (nvAgent.remainingDistance < 2)
            {
                nvAgent.ResetPath();
                ChangeState(State.IDLE);
            }
        }
    }
    private IEnumerator ATTACK()
    {
        if (!nvAgent.isStopped)
        {
            nvAgent.isStopped = true;
        }
        switch (monsterName)
        {
            case "Bear":
                AttackType attackType = _MonsterAnimator.AttackAnimation(monsterName);

                yield return new WaitForSeconds(_MonsterAnimator.StateInfo() - 1f);
                if (_hit.collider.name != null && attackType != AttackType.ButtAttack)
                {
                    Attack(_hit.collider.name, monsterName, attackType);
                }
                else if (attackType == AttackType.ButtAttack)
                {
                    Attack(_hit.collider.name, monsterName, attackType);
                }
                break;
        }

        yield return new WaitForSeconds(unitAbility.attackSpeed);
    }
    private IEnumerator DEATH()
    {
        _MonsterAnimator.DeathAnimation();
        yield return new WaitForSeconds(_MonsterAnimator.StateInfo() + 2f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (monsterType)
        {
            case MonsterType.hostility:
                switch (other.name)
                {
                    case "Player":
                        ChangeState(State.RUN);
                        StopAllCoroutines();
                        StartCoroutine(StateMachine());
                        break;
                }
                break;
            case MonsterType.neutrality:

                break;
            case MonsterType.friendly:
                switch (other.name)
                {
                    case "Player":
                        ChangeState(State.RUN);
                        StopAllCoroutines();
                        StartCoroutine(StateMachine());
                        break;
                }
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        switch (monsterType)
        {
            case MonsterType.hostility:
                switch (other.name)
                {
                    case "Player":
                        if (state == State.RUN)
                        {
                            target = other.transform;
                            nvAgent.SetDestination(target.position);
                        }
                        break;
                }
                break;
            case MonsterType.neutrality:

                break;
            case MonsterType.friendly:
                switch (other.name)
                {
                    case "Player":
                        if (!friendlyMove)
                        {
                            friendlyMove = true;
                        }
                        break;
                }
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch (monsterType)
        {
            case MonsterType.hostility:
                switch (other.name)
                {
                    case "Player":
                        nvAgent.SetDestination(transform.position);
                        target = null;
                        ChangeState(State.IDLE);
                        break;
                }
                break;
            case MonsterType.neutrality:
                break;
            case MonsterType.friendly:
                switch (other.name)
                {
                    case "Player":
                        friendlyMove = false;
                        break;
                }
                break;
        }
    }
    private void ChangeState(State newState)
    {
        state = newState;
    }
    private void FixedUpdate()
    {
        if (monsterType != MonsterType.friendly)
        {
            if (Physics.SphereCast(transform.position, transform.transform.lossyScale.x * rayExtent, transform.forward, out _hit, rayRange))
            {
                if (_hit.collider.name == "Player")
                {
                    switch (monsterName)
                    {
                        case "Bear":
                            nvAgent.stoppingDistance = 2.5f;
                            ChangeState(State.ATTACK);
                            break;
                        case "Fox":
                            break;
                    }
                }
            }
            else
            {
                if (target != null)
                {
                    ChangeState(State.RUN);
                }
            }
        }
    }
    protected void Attack(string damageName, string monsterName, AttackType type)
    {
        switch (monsterName)
        {
            case "Bear":
                switch (type)
                {
                    case AttackType.RightAttack:
                        Debug.Log("RightAttack");
                        break;
                    case AttackType.LeftAttack:
                        Debug.Log("LeftAttack");
                        break;
                    case AttackType.BiteAttack:
                        Debug.Log("BiteAttack");
                        break;
                    case AttackType.StrongAttack:
                        Debug.Log("StrongAttack");
                        break;
                    case AttackType.ButtAttack:
                        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
                        for (int i = 0; i < hitColliders.Length; i++)
                        {
                            if (hitColliders[i].name == "Player")
                            {
                                Debug.Log("ButtAttack");
                            }
                        }
                        break;
                }
                break;
            case "Fox":
                break;
        }
    }
    protected Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 20f;
        randomDirection += transform.position;

        NavMeshHit nvhit;
        if (NavMesh.SamplePosition(randomDirection, out nvhit, 20f, NavMesh.AllAreas))
        {
            return nvhit.position;
        }
        else
        {
            return transform.position;
        }
    }
    public void DamageHit()
    {
        _monsterDamageHit.Hit();
        _MonsterAnimator.HitAnimation();
    }
}
