using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ModifiedStalkerStateManager : MonoBehaviour
{
    [SerializeField] private AgentData stalkerData;
    [SerializeField] private float attackRadius;
    [SerializeField] private int initialTargetIndex;
    [SerializeField] private List<Transform> targetList = new List<Transform>();

    /* Pathfinding Data */
    private Path path;
    private Seeker seeker;
    private int currentWayPoint = 0;
    private float nextWaypointDistance = 1f;
    private Vector2 direction;
    private bool reachedEndOfPath = false;

    /* Target Data */
    private Transform playerTransform;
    private Transform targetTransform;
    private Vector2 lastPlayerPosition;
    private int targetIndex = 0;

    /* Agent Data */
    private Rigidbody2D rb;
    private Transform enemySpriteTransform;
    private float xEnemyScale;
    private float deceleration = 0.5f;
    private bool isWaiting = false; 
    private float waitTime = 2f;
    private bool isPlayerInRange = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private float attackWindow = 0.3f;
    private bool _isTakingDamage = false;
    private float forceOnDamage = 7f;
    private float stunTime = 0.2f;

    /* State Machine */
    public enum StalkerState
    {
        Roaming,
        Stalking,
        Attacking,
    }

    private int sceneState;
    private StalkerState currentState;

    /*
     * Subscribes to the OnDeathPlayer event in the PlayerStats script.
     * Invokes: PlayerDead().
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += SceneState;
        //PlayerStats.OnDeathPlayer += PlayerDead;
    }

    /* Unsubscribes from the OnDeathPlayer event in the PlayerStats script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState += SceneState;
        //PlayerStats.OnDeathPlayer -= PlayerDead;
        CancelInvoke();
    }

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        targetTransform = targetList[targetIndex];
        rb = GetComponent<Rigidbody2D>();
        enemySpriteTransform = transform.Find("SpriteEnemy").GetComponent<Transform>();
        xEnemyScale = enemySpriteTransform.localScale.x;

        currentState = StalkerState.Roaming;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    /* Stalker Finite State Machine. Controlled by CombatStateManager. */
    private void Update()
    {
        if (sceneState == 1)
        {
            FindPath();

            switch (currentState)
            {
                case StalkerState.Roaming:

                    if (reachedEndOfPath && !isWaiting)
                    {
                        StartCoroutine(WaitToRoam());
                    }

                    break;
            }
        }
        else if (sceneState == 2)
        {
            FindPath();

            switch (currentState)
            {
                case StalkerState.Roaming:

                    currentState = StalkerState.Stalking;
                    break;

                case StalkerState.Stalking:

                    if (Vector2.Distance(transform.position, targetTransform.position) < attackRadius)
                    {
                        isPlayerInRange = true;
                        lastPlayerPosition = playerTransform.position;
                        currentState = StalkerState.Attacking;
                    }
                    else
                    {
                        isPlayerInRange = false;
                        reachedEndOfPath = false;
                    }

                    break;

                case StalkerState.Attacking:

                    if (canAttack)
                    {
                        StartCoroutine(Attack());
                    }

                    break;
            }
        }
    }

    /* Controls agent movement. */
    private void FixedUpdate()
    {
        if (!_isTakingDamage)
        {
            if (!reachedEndOfPath && !isPlayerInRange)
            {
                rb.AddForce(direction * (stalkerData.MoveSpeed * 100) * Time.deltaTime);
            }
            else
            {
                /* Smooth deceleration. */
                if (rb.velocity.magnitude > 0 ^ currentState == StalkerState.Stalking)
                {
                    rb.velocity -= rb.velocity.normalized * deceleration * Time.deltaTime;

                    if (rb.velocity.magnitude < 0.01f)
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }
    }

    /* Assigns scene state from CombatStateManager. */
    private void SceneState(int state)
    {
        sceneState = state;

        switch (sceneState)
        {
            case 1:

                targetTransform = targetList[targetIndex];
                reachedEndOfPath = false;
                isWaiting = false;
                UpdatePath();
                currentState = StalkerState.Roaming;
                break;

            case 2:

                targetTransform = playerTransform;
                UpdatePath();
                break;
        }
    }

    /* ----------------------------- A STAR FUNCTIONS START --------------------------- */

    /* Updates the path based on the target's position. */
    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetTransform.position, OnPathComplete);
        }
    }

    /* Calculates the direction and distance of the next node in the path. */
    private void FindPath()
    {
        if (path == null) { return; }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        if (direction.x < 0)
        {
            Debug.Log("LEFT");
            enemySpriteTransform.localScale = new Vector3(-xEnemyScale, enemySpriteTransform.localScale.y, enemySpriteTransform.localScale.z);
        }
        else
        {
            Debug.Log("RIGHT");
            enemySpriteTransform.localScale = new Vector3(xEnemyScale, enemySpriteTransform.localScale.y, enemySpriteTransform.localScale.z);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }
    }

    /* Sets current way point to 0 when path is complete. */
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    /* ----------------------------- A STAR FUNCTIONS END ----------------------------- */

    /* 
     * Waiting Logic.
     * Only for Roaming State.
     */
    private IEnumerator WaitToRoam()
    {
        isWaiting = true;
        
        yield return new WaitForSeconds(waitTime);

        targetIndex++;
        targetIndex = (targetIndex > targetList.Count - 1) ? 0 : targetIndex;
        targetTransform = targetList[targetIndex];
        UpdatePath();
        reachedEndOfPath = false;

        Debug.Log("Reached End of Path: " + reachedEndOfPath);

        isWaiting = false;
    }

    /*
     * Attack Logic.  
     * Enemy dashes forward in the direction of the target.
     */
    private IEnumerator Attack()
    {
        canAttack = false;
        
        isAttacking = true;

        Vector2 directionToPlayer = (lastPlayerPosition - (Vector2)transform.position).normalized;
        rb.AddForce(directionToPlayer * stalkerData.DashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(attackWindow);

        isAttacking = false;

        //HERE'S THE EDITED BIT
        GetComponentInChildren<Animator>().SetTrigger("Attack");

        yield return new WaitForSeconds(stalkerData.DashCooldown);

        isPlayerInRange = false;
        reachedEndOfPath = false;
        currentState = StalkerState.Stalking;
        canAttack = true;
    }

    private IEnumerator Stun(Vector2 direction)
    {
        _isTakingDamage = true;
        rb.AddForce(-direction * forceOnDamage, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        _isTakingDamage = false;
    }

    /* Logic to register damage. */
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableObj = collision.gameObject.GetComponent<IDamageable>();

        if (damageableObj != null)
        {
            if (isAttacking)
            {
                damageableObj.TakeDamage(stalkerData.Attack);
            }

        }

        GameObject bullet = collision.gameObject;

        if (bullet != null)
        {
            Vector2 bulletPos = bullet.GetComponent<Transform>().position;
            Vector2 direction = (bulletPos - (Vector2)transform.position).normalized;
            Debug.Log(direction);
            StartCoroutine(Stun(direction));
        }
    }

    /* Gizmo for visibility. */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    /* Getter/Setter */
    public bool IsTakingDamage
    {
        get { return _isTakingDamage; }
        set { _isTakingDamage = value; }
    }
}