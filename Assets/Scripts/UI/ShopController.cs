using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    [SerializeField] Button restoreButton;
    [SerializeField] Button increaseButton;

    [SerializeField] GameObject[] lifeSprites;
    [SerializeField] TextMeshProUGUI moneyDisplay;

    public PlayerInventory inv;
    public PlayerStats stats;

    [SerializeField] ShopTrigger trig;
    [SerializeField] private GameObject crossFade;
    public int restoreState;
    

    private void Start()
    {
        inv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        trig.Clear();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            
            BackToCar();

        }
    }


    //restores player's health - either a set increment or up to the max
    public void RestoreHealth()
    {
        //for now, fully healing based on current max health
        stats.Heal(100);
        inv.Coins -= 15;
        UpdateMenu();
    }

    //increases the player's max health
    public void IncreaseMaxHealth()
    {
        inv.Coins -= 30;
        UpdateMenu();
    }

    //closes the shop menu
    public void BackToCar()
    {
        crossFade.SetActive(true);
        if (restoreState == 0)
        {
            CombatStateManager.current.SetState(CombatStateManager.SceneState.Friendly);
        }
        else
        {
            CombatStateManager.current.SetState(CombatStateManager.SceneState.Neutral);
        }
        Debug.Log("shop exit");
        gameObject.SetActive(false);
        trig.on = true;
    }

    //updates the shop menu
    public void UpdateMenu()
    {
        
        Debug.Log(inv.Coins);
        crossFade.SetActive(false);
        CombatStateManager.current.SetState(CombatStateManager.SceneState.Menu);

        //update shop option availability
        if (inv.Coins < 15 || stats.health>=100)//Not sure how to access max health, but that's the second check
        {
            increaseButton.interactable = false;
            increaseButton.GetComponent<Image>().color = Color.grey;

        }

        if (inv.Coins < 30)// ADD OR FROM PLAYER MAXHEALTH == CAP
        {
            increaseButton.interactable = false;
            increaseButton.GetComponent<Image>().color = Color.grey;
        }

        //update information displayed
        moneyDisplay.text = "Money: " + inv.Coins;

        //ADD UPDATE BASED ON PLAYER HEALTH
    }
}
