using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static Action<GameObject> OnInteraction;

    private GameObject _promptObj;
    private Animator promptAnimator;
    private TransitionController _controller;
    private bool canInteract;

    private void Awake()
    {
        _promptObj = transform.Find("Prompt").gameObject;
        promptAnimator = _promptObj.GetComponent<Animator>();
    }

    private void Update()
    {
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                /*
                 * Calls all functions subscribed to this event.
                 * Subscription: SceneController.
                 */
                CombatStateManager.current.SetState(CombatStateManager.SceneState.Transition);
                _controller.LoadNextScene(gameObject);
            }
        }
    }

    /* Plays prompt animation and enables player to interact. */
    public void Prompt(bool value)
    {
        promptAnimator.SetBool("Prompt", value);
        canInteract = promptAnimator.GetBool("Prompt");
    }

    /* Getter/Setter */
    public bool PromptState
    {
        get { return _promptObj.activeInHierarchy; }
        set { _promptObj.SetActive(value); }
    }

    public TransitionController Controller
    {
        get { return _controller; }
        set { _controller = value; }
    }
}
