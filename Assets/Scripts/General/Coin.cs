using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;

    private Rigidbody2D coinRigidBody;
    private float duration;

    private void Awake()
    {
        duration = animationCurve.keys[2].time;
        coinRigidBody = gameObject.GetComponent<Rigidbody2D>();

        /* Invokes StopMovements after 0.2 seconds to zero velocity after a force was added in the EnemyStats script. */
        Invoke("StopMovement", 0.2f);
    }

    private void OnDisable()
    {
        /* Stops Disable() from invoking after disabling object. */
        CancelInvoke();
    }

    private void Update()
    {
        /* 
         * Evaluates (returns) the animation curve's value (Y value of the graph) and applies the value
         * to the object's y rotation to infinitely rotate the object.
         */
        float curveValue = animationCurve.Evaluate(Time.time % duration);
        transform.rotation = Quaternion.Euler(transform.rotation.x, curveValue, transform.rotation.z);
    }

    private void StopMovement()
    {
        coinRigidBody.velocity = Vector2.zero;
    }

    /* 
     * If the collider that enters this object has a PlayerInventory script attached to it,
     * invoke AddCoin() on the object and destroy this game object.
     */
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory playerInventory = collision.gameObject.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.AddCoin();
            Destroy(gameObject);
        }
    }
}
