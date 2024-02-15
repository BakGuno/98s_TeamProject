using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    friendly,
    neutrality,
    hostility
}
[CreateAssetMenu(fileName = "UnitAbility", menuName = "New UnitAbility")]
public class UnitAbility : ScriptableObject
{
    [Header("Info")]
    public string UnitName;
    public MonsterType type;
    public int hp;
    public int stamina;
    [Header("Power")]
    public int offensePower;
    public int defensePower;
    [Header("Speed")]
    public float moveSpeed;
    public float attackSpeed;
    [Header("Range")]
    public float detectionRange;
    public float attackRange;
    public float ExtentRange;
}
