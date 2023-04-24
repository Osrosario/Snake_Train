using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Player Data")]
    [SerializeField] private AgentData playerData;

    public static Action<float> OnDamagePlayer;
    public static Action<float> OnHealPlayer;

    private float health;
    private Transform textOriginTransform;

    private void Start()
    {
        health = playerData.maxHealth;
        textOriginTransform = transform.Find("FloatingTextOrigin").GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }

    /* Adds the value to health and invokes OnHealPlayer event that the value to its own UI element. */
    public void Heal(float value)
    {
        health += value;

        if (health > playerData.maxHealth)
        {
            health = playerData.maxHealth;
        }

        float adjustedHealth = (health / playerData.maxHealth);

        /*
         * Calls all functions subscribed to this event.
         * Subscription: PlayerHealthBar.
         */
        OnHealPlayer?.Invoke(adjustedHealth);
    }

    /* Subtracts the damage from health and invokes OnHealPlayer event that the value to its own UI element. */
    public void TakeDamage(float value)
    {
        health -= value;
        float adjustedHealth = (health / playerData.maxHealth);

        ShowValue(value);

        /*
         * Calls all functions subscribed to this event.
         * Subscription: PlayerHealthBar.
         */
        OnDamagePlayer?.Invoke(adjustedHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ShowValue(float value)
    {
        GameObject floatingTextObj = TextPooler.current.GetPooledObject();
        floatingTextObj.GetComponentInChildren<TMP_Text>().text = value.ToString();
        floatingTextObj.transform.position = textOriginTransform.position;
        floatingTextObj.transform.rotation = Quaternion.identity;
        floatingTextObj.SetActive(true);
    }
}