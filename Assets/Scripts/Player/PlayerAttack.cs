using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private GunData revolverData;

    /* Weapon Data */
    private Transform leftFirePoint;
    private Transform rightFirePoint;
    private Animator leftWeaponAnimator;
    private Animator rightWeaponAnimator;
    private AudioSource weaponSFX;

    private int sceneState;
    private bool canFire = true;
    private bool fireFromLeft = true;

    private void Awake()
    {
        leftFirePoint = transform.Find("Revolver (Pivot)/FirePointLeft").GetComponent<Transform>();
        rightFirePoint = transform.Find("Revolver (Pivot)/FirePointRight").GetComponent<Transform>();
        leftWeaponAnimator = transform.Find("Revolver (Pivot)/WeaponLeft").GetComponent<Animator>();
        rightWeaponAnimator = transform.Find("Revolver (Pivot)/WeaponRight").GetComponent<Animator>();
        weaponSFX = transform.Find("Revolver (Pivot)").GetComponent<AudioSource>();
    }

    /*
     * Subscribes to the SendSceneState event in the CombatStateManager script.
     * Invokes: SceneState().
     */
    private void OnEnable()
    {
        CombatStateManager.SendSceneState += SceneState;
    }

    /* Unsubscribes from the SendSceneState event in the CombatStateManager script (if destroyed). */
    private void OnDisable()
    {
        CombatStateManager.SendSceneState += SceneState;
    }

    private void Update()
    {
        switch (sceneState)
        {
            case 1:
            case 2:
                
                if (Input.GetMouseButton(0))
                {
                    Fire();
                }

                break;
        }
    }

    /* 
     * Uses Object Pooler Design Pattern to enable and disable bullet objects. 
     * Gets bullet object from pooler, sets position, rotation, and force to rigidbody.
     */
    private void Fire()
    {
        if (canFire)
        {
            GameObject projectile = ObjectPooler.current.GetPooledObject<Bullet>();

            if (projectile != null)
            {
                if (fireFromLeft)
                {
                    leftWeaponAnimator.Play("r_Shot");
                    projectile.transform.position = leftFirePoint.transform.position;
                    projectile.transform.rotation = leftFirePoint.transform.rotation;
                }
                else
                {
                    rightWeaponAnimator.Play("f_Shot");
                    projectile.transform.position = rightFirePoint.transform.position;
                    projectile.transform.rotation = rightFirePoint.transform.rotation;
                }

                projectile.GetComponent<Bullet>().FireForce = revolverData.FireForce;
                int randomDamage = Mathf.FloorToInt(Random.Range(revolverData.MinDamage, revolverData.MaxDamage));
                projectile.GetComponent<Bullet>().BulletDamage = randomDamage;
                projectile.SetActive(true);
                weaponSFX.PlayOneShot(revolverData.SFX);
                StartCoroutine(FireTimer());
            }
            else
            {
                return;
            }
        }
    }

    /* Fire Rate Control. */
    private IEnumerator FireTimer()
    {
        canFire = false;
        yield return new WaitForSeconds(revolverData.FireRate);
        fireFromLeft = !fireFromLeft;
        canFire = true;
    }

    private void SceneState(int state) 
    { 
        sceneState = state; 
    }
}
