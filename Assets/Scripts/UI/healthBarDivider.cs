using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthBarDivider : MonoBehaviour
{
    //this script adds dividers to the player's health bar based on their max health
    [SerializeField] GameObject divider;

    // Start is called before the first frame update
    void Start()
    {
        float maxhp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().getMaxHealth();
        maxhp -= 100;
        int sec = (int)(maxhp / 25);
        for(int g = 0; g<sec; g++)
        {
            GameObject created = Instantiate(divider);
            created.transform.parent = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
