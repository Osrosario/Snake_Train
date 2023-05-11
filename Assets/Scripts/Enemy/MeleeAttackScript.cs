using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackScript : MonoBehaviour
{
    [SerializeField] EnemyStats stats;

    //This script is for an attack hitbox, dealing damage when triggered
    private void Start()
    {
        stats = GetComponentInParent<EnemyStats>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        Debug.Log(stats.meleeAttack());
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(stats.meleeAttack());
        } else if (collision.gameObject.CompareTag("Breakable"))
        {
            collision.gameObject.GetComponent<Breakable>().TakeDamage(stats.meleeAttack());
        }
    }
}
