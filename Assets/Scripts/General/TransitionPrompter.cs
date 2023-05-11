using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPrompter : MonoBehaviour
{
    [SerializeField] private TransitionController transitionController;
    private SpriteRenderer arrowSprend;
    private bool canOpen;

    /* Finds the child with the specified name of the object this script is attached to. */
    private void Awake()
    {
        arrowSprend = transform.Find("SpriteArrow").GetComponent<SpriteRenderer>();
    }

    /*
     * Subscribes to the CurrentStateState event in the CombatStateManager script.
     * Invokes: Open()
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += Open;
    }

    /* Unsubscribes from the CurrentStateState event in the CombatStateManager script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState -= Open;
    }

    /* Enables prompt, sends data to enable interaction, and sends object's instance of TransitionController. */
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();
            //edited by Callandra
            if (player != null)
            {
                player.PromptState = true;
                player.Prompt(canOpen);
                player.Controller = transitionController;
            }
        }
    }

    /* Disables prompt and sends data to disable interaction. */
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();
            if (player != null)
            {
                player.PromptState = false;
                player.Prompt(canOpen);
            }
        }
    }

    /* Enables player to interact with transition and sets color of the Arrow Sprite to green. */
    private void Open(int state)
    {
        if (state != 2)
        {
            canOpen = true;
            arrowSprend.color = ColorUtility.TryParseHtmlString("#00A619", out Color green) ? green : arrowSprend.color;
        }
        else
        {
            canOpen = false;
            arrowSprend.color = ColorUtility.TryParseHtmlString("#DB0600", out Color red) ? red : arrowSprend.color;
        }
    }

    /* Gizmos for visibility. */
    public void OnDrawGizmos()
    {
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
        Gizmos.DrawCube(gameObject.transform.position, new Vector3(collider.size.x, collider.size.y, 0f));
    }
}
