using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Core Player Data")]
    [SerializeField] private AgentData playerData;
    [SerializeField] private Camera sceneCamera;

    /* Core Player Data */
    private PlayerStats playerStats;
    private Rigidbody2D playerRigidBody;
    private Transform playerSpriteTransform;
    private int sceneState;

    /* Weapon Data */
    private Transform weaponTransform;

    /* Movement Data */
    private Vector2 movement;
    private Vector2 mousePosition;
    private float speed;
    private float xPlayerScale;
    private float yWeaponScale;
    private string _CapturedDirection;

    /* Dash Properties */
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private AudioSource dashSound;

    /* Finds the child with the specified name of the object this script is attached to. */
    private void Awake()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerSpriteTransform = transform.Find("SpritePlayer").GetComponent<Transform>();
        weaponTransform = transform.Find("Revolver (Pivot)").GetComponent<Transform>();
        dashSound = gameObject.GetComponent<AudioSource>();
    }

    /*
     * Subscribes to the SendSceneState event in the CombatStateManager script.
     * Invokes: SceneState().
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += SceneState;
    }

    /* Unsubscribes from the SendSceneState event in the CombatStateManager script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState += SceneState;
    }

    private void Start()
    {
        speed = playerData.MoveSpeed;
        xPlayerScale = playerSpriteTransform.localScale.x;
        yWeaponScale = weaponTransform.localScale.y;
    }

    /* Process User Input. Controlled by CombatStateManager. */
    private void Update()
    {
        switch (sceneState)
        {
            case 0:
            case 1:
            case 2:

                ProcessInputs();
                AimWeapon();
                break;

            case 3:

                if (CapturedDirection == "Left")
                {
                    playerSpriteTransform.localScale = new Vector3(-xPlayerScale, playerSpriteTransform.localScale.y, playerSpriteTransform.localScale.z);
                    weaponTransform.localScale = new Vector3(weaponTransform.localScale.x, -yWeaponScale, weaponTransform.localScale.z);
                    float currentRotation = weaponTransform.rotation.eulerAngles.z;
                    float newRotation = Mathf.MoveTowardsAngle(currentRotation, -180f, 500f * Time.deltaTime);
                    weaponTransform.rotation = Quaternion.Euler(0f, 0f, newRotation);
                }
                else
                {
                    playerSpriteTransform.localScale = new Vector3(xPlayerScale, playerSpriteTransform.localScale.y, playerSpriteTransform.localScale.z);
                    weaponTransform.localScale = new Vector3(weaponTransform.localScale.x, yWeaponScale, weaponTransform.localScale.z);
                    float currentRotation = weaponTransform.rotation.eulerAngles.z;
                    float newRotation = Mathf.MoveTowardsAngle(currentRotation, 0, 500f * Time.deltaTime);
                    weaponTransform.rotation = Quaternion.Euler(0f, 0f, newRotation);
                }

                break;
        } 
    }

    private void FixedUpdate()
    {
        Move();   
    }

    private void ProcessInputs()
    {
        /* Gets input data from WASD or Arrow Keys. */
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        /* Converts to the mouse position to a world point position in the scene. */
        mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);

        /* Captures data from movement and stores values when Left-Shift or (exclusive) Right Mouse Button is pressed. */
        if ((Input.GetKeyDown(KeyCode.LeftShift) ^ Input.GetMouseButtonDown(1)) && canDash)
        {
            Vector2 directionOnDash = movement;
            StartCoroutine(Dash(directionOnDash));
        }
    }

    /* Player Movement. Controlled by CombatStateManager. */
    private void Move()
    {        
        switch (sceneState)
        {
            case 0:
            case 1:
            case 2:

                if (!isDashing)
                {
                    playerRigidBody.velocity = movement.normalized * (playerData.MoveSpeed * 100) * Time.deltaTime;
                }

                break;
        }
    }

    /* Weapon Aiming and Sprite Control */
    private void AimWeapon()
    {
        Vector2 aimDirection = (mousePosition - (Vector2)transform.position).normalized;

        /* Calculates the angle between two points. */
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        /* Sets the angle of this object to face the mouse position. */
        weaponTransform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, aimAngle);

        /* Flips sprite based on the x position of the mouse. */
        if (aimDirection.x < 0)
        {
            playerSpriteTransform.localScale = new Vector3(-xPlayerScale, playerSpriteTransform.localScale.y, playerSpriteTransform.localScale.z);
            weaponTransform.localScale = new Vector3(weaponTransform.localScale.x, -yWeaponScale, weaponTransform.localScale.z);
        }
        else
        {
            playerSpriteTransform.localScale = new Vector3(xPlayerScale, playerSpriteTransform.localScale.y, playerSpriteTransform.localScale.z);
            weaponTransform.localScale = new Vector3(weaponTransform.localScale.x, yWeaponScale, weaponTransform.localScale.z);
        }
    }

    /* Dash Mechanic. Sets timer on Dash. */
    private IEnumerator Dash(Vector2 direction)
    {
        canDash = false;
        isDashing = true;
        dashSound.Play();
        playerStats.IsDamageable = false;
        playerRigidBody.AddForce(direction.normalized * playerData.DashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        playerStats.IsDamageable = true;
        yield return new WaitForSeconds(playerData.DashCooldown);
        canDash = true;
    }

    /* Getter/Setter */
    private void SceneState(int state)
    {
        sceneState = state;
    }
    
    public string CapturedDirection
    {
        get { return _CapturedDirection; }
        set { _CapturedDirection = value; }
    }
}
