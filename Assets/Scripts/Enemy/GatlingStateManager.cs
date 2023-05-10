using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingStateManager : MonoBehaviour
{
    [SerializeField] private GunData gatlingData;
    [SerializeField] private Transform gatlingTransform;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float rotationSpeed;

    //added by Callandra
    [SerializeField] public bool badgerBoss;
    [SerializeField] GameObject flipPoint;
    [SerializeField] GameObject returnPoint;
    [SerializeField] Animator badgerGunAnimator;

    /* Agent Data */
    private Transform enemySpriteTransform;
    private float xEnemyScale;
    private float yGatlingScale;
    private Transform targetTransform;
    private Vector3 lastTargetPosition;

    /* Weapon Data */
    private Transform firePointTransform;
    private Vector2 directionToTarget;
    private bool isPlayerInRange;
    private bool isPlayerDetected;
    private bool canFire = true;
    private float drumCapacity;

    private int sceneState;
    private GatlingState currentState;

    public enum GatlingState
    {
        Scanning,
        Firing,
        Inspecting,
        Reloading,
        Throwing
    }

    /*
     * Subscribes to the OnDeathPlayer event in the PlayerStats script.
     * Invokes: PlayerDead().
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += SceneState;
        PlayerStats.OnDeathPlayer += PlayerDead;
    }

    /* Unsubscribes from the OnDeathPlayer event in the PlayerStats script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState += SceneState;
        PlayerStats.OnDeathPlayer -= PlayerDead;
    }

    /* Finds the child with the specified name of the object this script is attached to. */
    private void Awake()
    {
        enemySpriteTransform = transform.Find("SpriteEnemy").GetComponent<Transform>();
        xEnemyScale = enemySpriteTransform.localScale.x;
        yGatlingScale = gatlingTransform.localScale.y;
        targetTransform = GameObject.Find("Player").GetComponent<Transform>();
        firePointTransform = transform.Find("Gatling (Pivot)/FirePoint").GetComponent<Transform>();
        drumCapacity = gatlingData.Capacity;

        /* Sets current state of object. */
        currentState = GatlingState.Scanning;
    }

    /* Gatling Finite State Machine. Controlled by CombatStateManager. */
    private void Update()
    {
        if (targetTransform != null)
        {
            directionToTarget = (targetTransform.position - gatlingTransform.position).normalized;
            DrawRay();
            Scan();
            AimWeapon();

            if (sceneState == 2)
            {
                switch (currentState)
                {
                    case GatlingState.Scanning:

                        currentState = (isPlayerInRange) ? GatlingState.Firing : GatlingState.Scanning;
                        break;

                    case GatlingState.Firing:

                        Firing();
                        break;

                    case GatlingState.Inspecting:

                        currentState = (drumCapacity != 0) ? GatlingState.Scanning : GatlingState.Reloading;
                        break;

                    case GatlingState.Reloading:

                        StartCoroutine(Reload());
                        break;

                    case GatlingState.Throwing:

                        ThrowMolotov();
                        break;
                }
            }
        }
    }

    /* Player Detection. */
    private void Scan()
    {
        if (Vector2.Distance(targetTransform.position, gatlingTransform.position) < detectionRadius)
        {
            isPlayerInRange = true;
            lastTargetPosition = targetTransform.position;
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    /* Weapon Aiming and Sprite Control */
    private void AimWeapon()
    {
        /* Calculates the angle between two points. */
        float aimAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        /* Sets the angle of this object to face the mouse position. */
        gatlingTransform.rotation = Quaternion.Lerp(gatlingTransform.rotation, Quaternion.Euler(0, 0, aimAngle), rotationSpeed * Time.deltaTime);

        if (directionToTarget.x < 0)
        {
            
            enemySpriteTransform.localScale = new Vector3(-xEnemyScale, enemySpriteTransform.localScale.y, enemySpriteTransform.localScale.z);

            /**<<<<<<< Updated upstream
                        gatlingTransform.localScale = new Vector3(gatlingTransform.localScale.x, -yGatlingScale, gatlingTransform.localScale.z);
=======
            **/
                        gatlingTransform.localScale = new Vector3(gatlingTransform.localScale.x, -yGatlingScale , gatlingTransform.localScale.z);


                        //added by Callandra
                        if (badgerBoss)
                        {
                            gatlingTransform.position = flipPoint.transform.position;
                        }
                    }
                    else
                    {
                        enemySpriteTransform.localScale = new Vector3(xEnemyScale, enemySpriteTransform.localScale.y, enemySpriteTransform.localScale.z);
                        gatlingTransform.localScale = new Vector3(gatlingTransform.localScale.x, yGatlingScale, gatlingTransform.localScale.z);

                        //added by Callandra
                        if (badgerBoss)
                        {
                            gatlingTransform.position = returnPoint.transform.position;
                        }
                    }
                }

                /* Debug for visibility */
            private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gatlingTransform.position, detectionRadius);
    }

    private void DrawRay()
    {
        float adjustedRadius = detectionRadius - Vector2.Distance(firePointTransform.position, gatlingTransform.position);
        RaycastHit2D hit = Physics2D.Raycast(firePointTransform.position, firePointTransform.right, adjustedRadius);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                isPlayerDetected = true;
                Debug.DrawRay(firePointTransform.position, firePointTransform.right * hit.distance, Color.red);
            }
            else
            {
                isPlayerDetected = false;
                Debug.DrawRay(firePointTransform.position, firePointTransform.right * hit.distance, Color.green);
            }
        }
        else
        {
            Debug.DrawRay(firePointTransform.position, firePointTransform.right * adjustedRadius, Color.green);
        }
    }

    /* Getter/Setter */
    private void PlayerDead()
    {
        isPlayerInRange = false;
    }

    private void SceneState(int state)
    {
        sceneState = state;
    }

    /* ---------------------- GATLING STATES ---------------------- */

    /* 
     * Uses Object Pooler Design Pattern to enable and disable bullet objects. 
     * Gets bullet object from pooler, sets position, rotation, and force to rigidbody.
     */
    private void Firing()
    {
        if (drumCapacity > 0)
        {
            if (canFire && isPlayerDetected)
            {
                GameObject projectile = ObjectPooler.current.GetPooledObject<Bullet>();

                if (projectile != null)
                {
                    projectile.transform.position = firePointTransform.position;
                    projectile.transform.rotation = firePointTransform.rotation;
                    projectile.GetComponent<Bullet>().FireForce = gatlingData.FireForce;
                    int randomDamage = Mathf.FloorToInt(Random.Range(gatlingData.MinDamage, gatlingData.MaxDamage));
                    projectile.GetComponent<Bullet>().BulletDamage = randomDamage;
                    projectile.SetActive(true);
                    drumCapacity--;
                    StartCoroutine(FireTimer());

                    //added by Callandra
                    if (badgerBoss)
                    {
                        badgerGunAnimator.SetBool("Firing", true);
                    }
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            currentState = GatlingState.Throwing;
        }
    }
    
    /* Fire Rate Control. */
    private IEnumerator FireTimer()
    {
        canFire = false;
        yield return new WaitForSeconds(gatlingData.FireRate);
        canFire = true;
    }

    /* Reload Control. */
    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(gatlingData.ReloadSpeed);
        drumCapacity = gatlingData.Capacity;
        currentState = GatlingState.Scanning;
    }

    /* Molotov Control 
     * 
     * Uses Object Pooler Design Pattern to enable and disable bullet objects. 
     * Gets moolotov object from pooler, sets start position, end position, rotation.
     */
    private void ThrowMolotov()
    {
        GameObject throwable = ObjectPooler.current.GetPooledObject<Molotov>();

        if (throwable != null)
        {
            throwable.GetComponent<Molotov>().StartPosition = transform.position;
            throwable.GetComponent<Molotov>().EndPosition = lastTargetPosition;
            throwable.transform.rotation = Quaternion.identity;
            throwable.transform.localScale = gatlingTransform.localScale;
            throwable.SetActive(true);

            currentState = GatlingState.Inspecting;
        }
        else
        {
            return;
        }
    }

    /* Getter/Setter */
    public bool IsPlayerDetected
    {
        get { return isPlayerInRange; }
        set { isPlayerInRange = value; }
    }

    public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }
}