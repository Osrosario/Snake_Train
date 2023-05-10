using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Agent Data")]
    [SerializeField] private AgentData enemyData;
    [SerializeField] private Color damageColor;
    
    [Header("Enemy Loot")]
    [SerializeField] private GameObject coinObject;
    [SerializeField] int amountToDrop;

    public static Action OnSpawn;
    public static Action OnDestroy;

    private SpriteRenderer enemySprend;
    private GameObject enemyHealthBarObj;
    //private WorldSpaceHealthBar enemyHealthBar;
    private Transform textOriginTransform;
    private float health;
    private bool isTakingDamage = false;
    private float flickerTime = 0.05f;
    private Color orignalColor;
    
    private void Awake()
    {
        health = enemyData.MaxHealth;
        enemyHealthBarObj = transform.Find("Canvas/HealthBarEnemy").gameObject;
        //enemyHealthBar = enemyHealthBarObj.GetComponent<WorldSpaceHealthBar>();
        enemyHealthBarObj.SetActive(false);
        textOriginTransform = transform.Find("FloatingTextOrigin").GetComponent<Transform>();
        enemySprend = transform.Find("SpriteEnemy").GetComponent<SpriteRenderer>();
        orignalColor = enemySprend.color;
    }

    private void Start()
    {
        /*
        * Calls all functions subscribed to this event.
        * Subscription: CombatStateManager.
        */
        OnSpawn?.Invoke();
    }

    /* Subtracts the damage from health and calls a function that sends the remaining health to its own UI element. */
    public void TakeDamage(float value)
    {
        health -= value;

        if (!isTakingDamage)
        {
            StartCoroutine(Damaged());
        }

        /* Sets the enemy health bar to active once it recieves damage. */
        if (health != enemyData.MaxHealth)
        {
            enemyHealthBarObj.SetActive(true);
            CombatStateManager.current.SetState(CombatStateManager.SceneState.Hostile);
        }

        ShowValue(value);

        float adjustedHealth = (health / enemyData.MaxHealth);
        //enemyHealthBar.ShowDamage(adjustedHealth);

        if (health <= 0)
        {
            /*
             * Calls all functions subscribed to this event.
             * Subscription: EnemyHandler.
             */
            OnDestroy?.Invoke();

            DropCoins();
            Destroy(gameObject);
        }
    }

    /* Retrieves text object from TextPooler and applies properties. */
    private void ShowValue(float value)
    {
        GameObject floatingTextObj = ObjectPooler.current.GetPooledObject<FloatingText>();
        floatingTextObj.GetComponentInChildren<TMP_Text>().text = value.ToString();
        floatingTextObj.transform.position = textOriginTransform.position;
        floatingTextObj.transform.rotation = Quaternion.identity;
        floatingTextObj.SetActive(true);
    }

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

    /* Sprite flicker to show damage. */
    private IEnumerator Damaged()
    {
        isTakingDamage = true;
        
        enemySprend.color = damageColor;
        yield return new WaitForSeconds(flickerTime);
        enemySprend.color = orignalColor;

        isTakingDamage = false;
    }
}
