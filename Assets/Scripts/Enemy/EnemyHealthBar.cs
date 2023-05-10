using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : Healthbar
{
    private void Awake()
    {
        fill = transform.Find("Fill").GetComponent<Image>();
        damageFill = transform.Find("FillDamage").GetComponent<Image>();
    }
}
