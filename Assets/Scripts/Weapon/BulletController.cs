using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The class is attached to a Pooled Object and used inconjuction with an Object Pooler. This script
 * is attached to the objects stored in the Object Pooler class. In this case, this script is attached
 * to the Bullet object which is stored in a list in the BulletPooler class. Once a class has retrived
 * a bullet object and enables it, a force will be applied and Disable() will be invoked for 2 seconds.
 * Once it's disabled, the bullet will disappear if hasn't collided with other objects, simulating a 
 * destroyed object. Since the bullet objects have been pre-instantiated, classes can resuse the same 
 * object and re-enable them as the same force can be applied once they set them active. This creates
 * a seemingly infinite amount of bullet objects without taxing the user's system.
 */
public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bulletRigidBody;

    private float _fireForce;
    private float _bulletDamage;

    private void OnEnable()
    {
        /* Resets velocity on rigid body after re-enabling. */
        if (bulletRigidBody != null)
        {
            bulletRigidBody.AddForce(transform.right * _fireForce, ForceMode2D.Impulse);
        }
        
        /* Invokes Disable(). Disables object after 2 seconds. */
        Invoke("Disable", 2f);
    }

    private void Start()
    {
        bulletRigidBody.AddForce(transform.right * _fireForce, ForceMode2D.Impulse);
    }

    private void OnDisable()
    {
        /* Stops Disable() from invoking after disabling object. */
        CancelInvoke();
    }

    /* Disables object depending on what it collides with. */
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        
        switch (collision.gameObject.tag)
        {
            case "Player":
                collision.gameObject.GetComponent<PlayerStats>().TakeDamage(_bulletDamage);
                Disable();
                break;
            case "Enemy":
                collision.gameObject.GetComponent<EnemyStats>().TakeDamage(_bulletDamage);
                Disable();
                break;
            case "Obstacle":
                Disable();
                break;
            case "Breakable":
                collision.gameObject.GetComponent<EnemyStats>().TakeDamage(_bulletDamage);
                Disable();
                break;
        }
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    /* Getters/Setters*/
    public float FireForce
    {
        get { return _fireForce; }
        set { _fireForce = value; }
    }

    public float BulletDamage
    {
        get { return _bulletDamage; }
        set { _bulletDamage = value; }
    }
}
