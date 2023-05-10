using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    /* 
     * This holds the player's inventory. Currently all that we have in here is gold, but we may later add gun types and/or ammo. 
     * It also holds the gold prefabs, for the moment.
     */
    [SerializeField] private InventoryData playerInventory;

    void Start()
    {
        playerInventory.Coins = 0;
    }

    public void AddCoin()
    {
        playerInventory.Coins += 1;
    }

    public void SubtractCoins(int remove)
    {
        playerInventory.Coins -= remove;
    }

    public int ReturnCoins()
    {
        return playerInventory.Coins;
    }
}
