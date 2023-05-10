using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{

    // referenced code from https://answers.unity.com/questions/260100/instantiate-as-a-child-of-the-parent.html

    [SerializeField] Button restoreButton;
    [SerializeField] Button increaseButton;

    [SerializeField] GameObject barDivHolder;
    [SerializeField] TextMeshProUGUI moneyDisplay;
    [SerializeField] GameObject healthDiv;
    

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
        stats.Heal(200);
        //inv.Coins -= 15;
        inv.SubtractCoins(15);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().Heal(200);
        UpdateMenu();
    }

    //increases the player's max health
    public void IncreaseMaxHealth()
    {
        //inv.Coins -= 30;
        inv.SubtractCoins(30);
        stats.IncreaseMaxHealth(25);
        GameObject created = Instantiate(healthDiv);
        created.transform.parent = barDivHolder.transform;
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

        //Debug.Log(inv.Coins);
        Debug.Log(inv.ReturnCoins());
        crossFade.SetActive(false);
        CombatStateManager.current.SetState(CombatStateManager.SceneState.Menu);

        //update shop option availability
        if (inv.ReturnCoins() < 15 || stats.IsHealthFull() ==  true)//Not sure how to access max health, but that's the second check
        {
            restoreButton.interactable = false;
            restoreButton.GetComponent<Image>().color = Color.grey;

        }
        else
        {
            restoreButton.interactable = true;
            restoreButton.GetComponent<Image>().color = Color.white;
        }

        if (inv.ReturnCoins() < 30)// ADD OR FROM PLAYER MAXHEALTH == CAP
        {
            increaseButton.interactable = false;
            increaseButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            increaseButton.interactable = true;
            increaseButton.GetComponent<Image>().color = Color.white;
        }

        //update information displayed
        moneyDisplay.text = "Money: " + inv.ReturnCoins();

        //ADD UPDATE BASED ON PLAYER HEALTH
    }
}
