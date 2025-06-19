using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private SceneData sceneData;
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float doorSpeed;
    [SerializeField] private ExitDirection exitDirection;

    enum ExitDirection
    {
        Left,
        Right
    }

    /* Cross Fade Data */
    private Animator crossFadeAnimator;
    private float animationDuration;
    private bool canLoadNextScene;

    /* Transition Data */
    private Transform doorTransform;
    private Vector2 doorClosedPosition;
    private Vector2 doorOpenPosition;

    /* Finds the child with the specified name of the object this script is attached to. */
    private void Awake()
    {
        crossFadeAnimator = GameObject.Find("CrossFade (Canvas)").GetComponent<Animator>();
        doorTransform = transform.Find("Door").GetComponent<Transform>();
        doorClosedPosition = doorTransform.position;
        doorOpenPosition = new Vector2(doorClosedPosition.x, -(doorTransform.localScale.y * 0.925f));
        
        /* Gets the duration of FadeIn duration. Both FadeIn and FadeOut should be the same. */
        animationDuration = crossFadeAnimator.runtimeAnimatorController.animationClips[0].length;
    }

    /*
     * Subscribes to the CurrentStateState event in the CombatStateManager script.
     * Invokes: LoadState()
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += LoadState;
    }

    /* Unsubscribes from the CurrentStateState event in the CombatStateManager script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState -= LoadState;
    }

    /* If CombatStateManager is not in a Hostile State, enable scene transition. */
    private void LoadState(int state) 
    {
        if (state != 2)
        {
            canLoadNextScene = true;
        }
        else
        {
            canLoadNextScene = false;
        }
    }

    public void LoadNextScene(GameObject playerObject) 
    { 
        if (canLoadNextScene)
        {
            StartCoroutine(LoadScene(playerObject));
        }    
    }

    public void OnTriggerEnter2D()
    {
        GameObject.Find("ShopTrigger").GetComponent<ShopTrigger>().on = false;
    }
    
    /* 
     * Transition Animations
     * 
     * Triggers the FadeOut animation. After the animation is done, the scene changes to the LoadScreen 
     * scene and transitions to the desired scene.
     */
    private IEnumerator LoadScene(GameObject playerObject)
    {
        Rigidbody2D plyrRigidBody = playerObject.GetComponent<Rigidbody2D>();
        plyrRigidBody.velocity = Vector2.zero;
        Vector2 targetPosition;

        if (exitDirection == ExitDirection.Left)
        {
            targetPosition = new Vector2(transform.position.x - 2.5f, transform.position.y);
        }
        else
        {
            targetPosition = new Vector2(transform.position.x + 2.5f, transform.position.y);
        }

        playerObject.GetComponent<PlayerMovement>().CapturedDirection = exitDirection.ToString();
        playerObject.GetComponent<PlayerInteract>().Prompt(false);
        playerObject.GetComponent<PlayerInteract>().PromptState = false;
        //playerObject.GetComponent<CapsuleCollider2D>().enabled = false;
        GameObject.Find("Walls").gameObject.SetActive(false);

        /* Move player to Transition Controller position. */
        while (plyrRigidBody.position != (Vector2)transform.position)
        {
            plyrRigidBody.position = Vector2.MoveTowards(plyrRigidBody.position, transform.position, playerSpeed * Time.deltaTime);
            yield return null;
        }

        /* Open door. */
        while ((Vector2)doorTransform.position != doorOpenPosition)
        {
            doorTransform.position = Vector2.MoveTowards(doorTransform.position, doorOpenPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("move player?");
        /* Move player to the other side. */
        while (plyrRigidBody.position != targetPosition)
        {
            plyrRigidBody.position = Vector2.MoveTowards(plyrRigidBody.position, targetPosition, playerSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("close door?");
        /* Close door. */
        while ((Vector2)doorTransform.position != doorClosedPosition)
        {
            doorTransform.position = Vector2.MoveTowards(doorTransform.position, doorClosedPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("hit here!");

        /* Fade to black. Change Scene. */
        crossFadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(animationDuration);
        sceneData.SceneToLoad = nextSceneIndex;
        //edited by Callandra (from loadscene(0)
        SceneManager.LoadScene(1);
    }
}
