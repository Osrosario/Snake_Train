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
    public static Action OnDeathPlayer;

    public float health;
    private Transform textOriginTransform;

    private void Start()
    {
        health = playerData.MaxHealth;
        textOriginTransform = transform.Find("FloatingTextOrigin").GetComponent<Transform>();
    }

    /* Adds the value to health and invokes OnHealPlayer event that the value to its own UI element. */
    public void Heal(float value)
    {
        health += value;

        /* Checks if the health is over the max health of the agent. */
        if (health > playerData.MaxHealth)
        {
            health = playerData.MaxHealth;
        }

        float adjustedHealth = (health / playerData.MaxHealth);

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
        float adjustedHealth = (health / playerData.MaxHealth);

        ShowValue(value);

        /*
         * Calls all functions subscribed to this event.
         * Subscription: PlayerHealthBar, ConversationStarter.
         */
        OnDamagePlayer?.Invoke(adjustedHealth);

        if (health <= 0)
        {
            /*
             * Calls death function when health is 0.
             * Subscription: ConversationStarter.
             */
            OnDeathPlayer?.Invoke();
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

    //increases the player's max health
    public void IncreaseMaxHealth(int increase)
    {
        playerData.MaxHealth += increase;
        Heal(increase);
    }

    public bool CheckHealthPercentage()
    {
        bool full = false;
        if(health == playerData.MaxHealth)
        {
            full = true;
        }
        return full;
    }
}
