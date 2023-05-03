using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class BreakableScript : MonoBehaviour
{

    //this script just exists to destroy a breakable object when it's hit

    private Transform textOriginTransform;

    private void Start()
    {
        //this line is also copied from EnemyStats
        textOriginTransform = transform.Find("FloatingTextOrigin").GetComponent<Transform>();
    }

    //when hit, show damage done from bullet and destroy, triggering drops (if any)
    public void Hit(float value)
    {
        ShowValue(value);
        Destroy(gameObject);
    }

    //This method is copied from EnemyStats

    /* Retrieves text object from TextPooler and applies properties. */
    private void ShowValue(float value)
    {
        GameObject floatingTextObj = TextPooler.current.GetPooledObject();
        floatingTextObj.GetComponentInChildren<TMP_Text>().text = value.ToString();
        floatingTextObj.transform.position = textOriginTransform.position;
        floatingTextObj.transform.rotation = Quaternion.identity;
        floatingTextObj.SetActive(true);
    }
}
