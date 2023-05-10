using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private Animator textAnimator;

    private void Awake()
    {
        textAnimator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        /* Resets velocity on rigid body after re-enabling. */
        textAnimator.Play("FloatingText");

        /* Invokes Disable(). Disables object after 2 seconds. */
        Invoke("Disable", 2f);
    }

    private void Start()
    {
        textAnimator.Play("FloatingText");
    }

    private void OnDisable()
    {
        /* Stops Disable() from invoking after disabling object. */
        CancelInvoke();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
