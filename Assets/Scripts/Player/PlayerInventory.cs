using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    /* 
     * This holds the player's inventory. Currently all that we have in here is gold, but we may later add gun types and/or ammo. 
     * It also holds the gold prefabs, for the moment.
     */
    [SerializeField] private ItemData coinData;
    //[SerializeField] private int coinData;
    //public int coinCount;

    void Start()
    {
        coinData.Quantity = 0;
        //coinCount = 0;
        //coinData = 0;
    }

    /* Getter/Setter */
    /**
    public int Coins
    {
       
        
        get {
            Debug.Log("get coins!");
            return coinData.Quantity; }
        set {
            Debug.Log("set coins!");
            coinData.Quantity += value; }
        get {
            Debug.Log("get coins!");
            return coinData; }
        set {
            Debug.log("set coins!");
            coinData += value; }
}**/

    public void AddCoin()
    {
        coinData.Quantity += 1;
    }

    public void SubtractCoins(int remove)
    {
        coinData.Quantity -= remove;
    }

    public int ReturnCoins()
    {
        return coinData.Quantity;
    }
}
