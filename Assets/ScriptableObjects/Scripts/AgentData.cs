using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agent")]
public class AgentData : ScriptableObject
{
    public float MaxHealth;
    [Header("Mutiplied by 100")]
    public float MoveSpeed;
    public float DashForce;
    public float DashCooldown;
    [Header("Melee Agents Only")]
    public float Attack;
}
