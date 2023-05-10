using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private ThrowableData molotavData;
    [SerializeField] private LayerMask damagableLayerMask;

    private Animator molotovAnimator;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float travelSpeed;
    private float distance;
    private float maxHeight;
    private float startTime;

    private void Awake()
    {
        molotovAnimator = gameObject.GetComponent<Animator>();
        travelSpeed = molotavData.TravelSpeed;
    }

    private void OnEnable()
    {
        distance = Vector2.Distance(_startPosition, _endPosition);
        maxHeight = distance / 2;

        /* Plays animation depending on the y rotation of the object. */
        if (gameObject.transform.localScale.y < 0)
        {
            molotovAnimator.Play("SpinLeft");
        }
        else
        {
            molotovAnimator.Play("SpinRight");
        }

        startTime = Time.time;
    }

    void Update()
    {
        float timeSinceStart = Time.time - startTime;
        float distanceCovered = timeSinceStart * travelSpeed;
        float timeOverDistance = distanceCovered / distance;

        Vector2 currentPos = Vector2.Lerp(_startPosition, _endPosition, timeOverDistance);
        float height = curve.Evaluate(timeOverDistance) * maxHeight;
        currentPos.y += height;

        transform.position = currentPos;

        if (transform.position == new Vector3(_endPosition.x, _endPosition.y, transform.position.z))
        {
            Explode();
        }
    }

    /*
     * Collects all colliders entered and calls their TakeDamage().
     * Called by an event in the SpinLeft and SpinRight animation of the Molotov object.
     */
    public void Explode()
    {
        /* Collects an object's collider (if part of the "Damageable" layer mask) that was entered and stores them in a list */
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, molotavData.BlastRadius, damagableLayerMask);

        /* Goes through each collider in the list and calls their TakeDamage(). */ 
        foreach (Collider2D obj in objectsHit)
        {
            int randomDamage = Mathf.FloorToInt(Random.Range(molotavData.MinDamage, molotavData.MaxDamage));
            obj.GetComponent<IDamageable>().TakeDamage(randomDamage);
        }

        gameObject.SetActive(false);
    }

    /* Gizmos for visibility */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, molotavData.BlastRadius);
    }

    /* Getters/Setters */
    public Vector3 StartPosition
    {
        get { return _startPosition; }
        set { _startPosition = value; }
    }

    public Vector3 EndPosition
    {
        get { return _endPosition; }
        set { _endPosition = value; }
    }
}
