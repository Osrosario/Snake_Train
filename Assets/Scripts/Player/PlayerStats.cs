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

    private float maxHealth;
    private float health;
    private bool _isDamageable = true;
    private Transform textOriginTransform;

    private void Start()
    {
        maxHealth = playerData.MaxHealth;
        health = maxHealth;
        textOriginTransform = transform.Find("FloatingTextOrigin").GetComponent<Transform>();
    }

    /* Adds the value to health and invokes OnHealPlayer event that the value to its own UI element. */
    public void Heal(float value)
    {
        health += value;

        /* Checks if the health is over the max health of the agent. */
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        float adjustedHealth = (health / maxHealth);

        /*
         * Calls all functions subscribed to this event.
         * Subscription: PlayerHealthBar.
         */
        OnHealPlayer?.Invoke(adjustedHealth);
    }

    /* Subtracts the damage from health and invokes OnHealPlayer event that the value to its own UI element. */
    public void TakeDamage(float value)
    {
        if (_isDamageable)
        {
            health -= value;
            float adjustedHealth = (health / maxHealth);

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
    }

    private void ShowValue(float value)
    {
        GameObject floatingTextObj = ObjectPooler.current.GetPooledObject<FloatingText>();
        floatingTextObj.GetComponentInChildren<TMP_Text>().text = value.ToString();
        floatingTextObj.transform.position = textOriginTransform.position;
        floatingTextObj.transform.rotation = Quaternion.identity;
        floatingTextObj.SetActive(true);
    }

    //increases the player's max health
    public void IncreaseMaxHealth(int increase)
    {
        maxHealth += increase;
        Heal(increase);
    }

    public bool IsHealthFull()
    {
        bool isFull = (health < maxHealth) ? false : true;
        return isFull;
    }

    /* Getter/Setter */
    public bool IsDamageable
    {
        get { return _isDamageable; }
        set { _isDamageable = value; }
    }
}
