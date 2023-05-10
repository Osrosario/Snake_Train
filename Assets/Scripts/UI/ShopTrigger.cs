using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    /* This script controlls a box that on trigger prompts the player to open the shop screen. */
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject shopPrompt;
    [SerializeField] private GameObject shopAlert;
    [SerializeField] public bool on;
    [SerializeField] private bool hasOpened;
    private int sceneState;

    private void OnEnable()
    {
        CombatStateManager.SendSceneState += SceneState;


    }

    private void OnDisable()
    {
        CombatStateManager.SendSceneState -= SceneState;
    }

    // Start is called before the first frame update
    void Start()
    {
        on = false;
        hasOpened = false;
        shopAlert.SetActive(false);
    }

    public void Clear()
    {
        shopScreen.SetActive(false);
        shopPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasOpened == false)
        {
            if (sceneState == 0 || sceneState == 1)
            {

                    shopAlert.SetActive(true);
            }
        }
        
        if(on == true && Input.GetKeyDown(KeyCode.F))
        {
            shopScreen.SetActive(true);
            shopScreen.GetComponent<ShopController>().restoreState = sceneState;
            shopScreen.GetComponent<ShopController>().UpdateMenu();
            on = false;
            shopAlert.SetActive(false);
           
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(sceneState);
        if(sceneState == 0 || sceneState == 1)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                on = true;
                hasOpened = true;
                shopPrompt.SetActive(true);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            on = false;
            shopPrompt.SetActive(false);
        }
    }

    private void SceneState(int state)
    {
        sceneState = state;

        
        
    }
}
