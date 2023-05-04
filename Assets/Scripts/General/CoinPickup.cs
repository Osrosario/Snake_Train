using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    /* 
     * This script attatches to the gold pickup prefabs. On contact with player, add gold 
     * to inventory and either set active or destroy self
     */

    private GameObject Player;
    private bool trig;

    private void Start()
    {
        Player = null;
        trig = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("coin contact");
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
            if (trig == false)
            {
                trig = true;
                Player = other.gameObject;
                //other.gameObject.GetComponent<PlayerInventory>().Coins+=1;
                other.gameObject.GetComponent<PlayerInventory>().AddCoin();
                Destroy(gameObject);
            }

        }
    }

    private void OnDestroy()
    {
        if(Player != null)
        {
            //Player.GetComponent<PlayerInventory>().Coins += 1;
        }
       
    }
}
