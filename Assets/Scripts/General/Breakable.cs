using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Breakable : MonoBehaviour, IDamageable
{
    [Header("Core Data")]
    [SerializeField] private float health;
    [SerializeField] private GameObject coinObject;

    [Header("Loot")]
    [SerializeField] int amountToDrop;

    [Header("Sound SFX")]
    [SerializeField] private AudioSource objectAudioSource;
    [SerializeField] private AudioClip impactSFX1;
    [SerializeField] private AudioClip impactSFX2;

    private GameObject healthBarObj;
    private WorldSpaceHealthBar healthBar;
    private AudioClip currentSFX;
    private bool switcher;

    private void Awake()
    {
        //healthBarObj = transform.Find("Canvas/HealthBar").gameObject;
        //healthBar = healthBarObj.GetComponent<WorldSpaceHealthBar>();

        //healthBarObj.SetActive(false);
    }

    /* Switches when sound SFX when taking damage. Drops coins when destroyed. */
    public void TakeDamage(float damage)
    {
        currentSFX = (switcher) ? impactSFX1 : impactSFX2;
        objectAudioSource.PlayOneShot(currentSFX);

        health -= damage;

        if (health / 100 != 1)
        {
            //healthBarObj.SetActive(true);
        }

        float adjustedHealth = (health / 100);
        //healthBar.ShowDamage(adjustedHealth);
        switcher = !switcher;

        if (health <= 0)
        {
            DropCoins();
            Destroy(gameObject);
        }
    }

    /* Sets the position, a direction, and force to the object. */
    private void DropCoins()
    {
        for (int i = 0; i < amountToDrop; i++)
        {
            Vector2 direction = (Vector2)transform.position - Random.insideUnitCircle * 1;
            float randomForce = Mathf.CeilToInt(Random.Range(1f, 5f));

            GameObject newCoinObj = Instantiate(coinObject);
            newCoinObj.transform.position = transform.position;
            newCoinObj.GetComponent<Rigidbody2D>().AddForce(direction * randomForce, ForceMode2D.Impulse);
        }
    }
}
