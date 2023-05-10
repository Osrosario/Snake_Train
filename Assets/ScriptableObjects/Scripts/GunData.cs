using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun")]
public class GunData : ScriptableObject
{
    public AudioClip SFX;
    public float MaxDamage;
    public float MinDamage;
    public float FireForce;
    public float FireRate;
    public float Capacity;
    public float ReloadSpeed;
}
