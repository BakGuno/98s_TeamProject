using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static MonsterAnimation;
using static UnityEditor.FilePathAttribute;

public class MonsterController : MonoBehaviour, IDamagable
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

    protected IDamagable damagable;

    [HideInInspector]
    public string monsterName;
    protected int hp;
    protected int maxhp;
    protected int ad;
    protected int df;
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
        maxhp = unitAbility.hp;
        ad = unitAbility.offensePower;
        df = unitAbility.defensePower;
        adSpeed = unitAbility.attackSpeed;
        monsterType = unitAbility.type;
        _collider.radius = unitAbility.detectionRange;
        rayRange = unitAbility.attackRange;
        rayExtent = unitAbility.ExtentRange;

        nvAgent.speed = unitAbility.moveSpeed;
        hp -= 1;
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
        _rigidbody.velocity = Vector3.zero;

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
        AttackType attackType = _MonsterAnimator.AttackAnimation(monsterName);
        yield return new WaitForSeconds(_MonsterAnimator.StateInfo());
        if (_hit.collider != null && _hit.collider.name == "Player")
        {
            Attack(monsterName, attackType);
        }
        else if (_hit.collider == null || _hit.collider.name != "Player")
        {
            ChangeState(State.RUN);
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
                switch (other.name)
                {
                    case "Player":
                        if (hp < maxhp)
                        {
                            ChangeState(State.RUN);
                            StopAllCoroutines();
                            StartCoroutine(StateMachine());
                        }
                        break;
                }
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
                switch (other.name)
                {
                    case "Player":
                        if (hp < maxhp)
                        {
                            if (state == State.RUN)
                            {
                                target = other.transform;
                                nvAgent.SetDestination(target.position);
                            }
                        }
                        break;
                }
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
                if (hp < maxhp)
                {
                    switch (other.name)
                    {
                        case "Player":
                            nvAgent.SetDestination(transform.position);
                            target = null;
                            ChangeState(State.IDLE);
                            break;
                    }
                }
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
        if (state != State.IDLE)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        if (monsterType != MonsterType.friendly)
        {
            if (Physics.SphereCast(transform.position, transform.transform.lossyScale.x * rayExtent, transform.forward, out _hit, rayRange))
            {
                if (_hit.collider.name == "Player")
                {
                    damagable = _hit.collider.GetComponent<IDamagable>();
                    switch (monsterName)
                    {
                        case "Bear":
                            ChangeState(State.ATTACK);
                            break;
                        case "Fox":
                            ChangeState(State.ATTACK);
                            break;
                    }
                }
            }
        }
    }
    protected void Attack(string monsterName, AttackType type)
    {
        switch (monsterName)
        {
            case "Bear":
                switch (type)
                {
                    case AttackType.RightAttack:
                        damagable.TakePhysicalDamage(ad);
                        break;
                    case AttackType.LeftAttack:
                        damagable.TakePhysicalDamage(ad);
                        break;
                    case AttackType.BiteAttack:
                        damagable.TakePhysicalDamage(ad + 10);
                        break;
                    case AttackType.StrongAttack:
                        damagable.TakePhysicalDamage(ad + 20);
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
                switch (type)
                {
                    case AttackType.RightAttack:
                        damagable.TakePhysicalDamage(ad);
                        break;
                    case AttackType.LeftAttack:
                        damagable.TakePhysicalDamage(ad + 10);
                        break;
                }
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
    public void DamageHit(int damage)
    {
        hp -= Math.Clamp((damage - df), 0, 999);
        if (monsterName == "Bear")
        {
            _MonsterAnimator.HitAnimation();
        }
        _monsterDamageHit.Hit();
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        DamageHit(damageAmount);
    }
}
