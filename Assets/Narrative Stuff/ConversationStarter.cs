using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PixelCrushers.DialogueSystem;

public class ConversationStarter : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private DialogueSystemTrigger playerDeathTrigger;
    [SerializeField] private DialogueSystemTrigger startTrigger;
    private void OnEnable()
    {
        deathCanvas.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        EnemyHandler.OnEnemiesDefeated += EnemyStateReset;
        PlayerStats.OnDamagePlayer += PlayerDamaged;
        PlayerStats.OnDeathPlayer += PlayerDeath;
        DialogueManager.instance.WarmUpConversationController();
        Invoke("StartConvo", 0.6f);
    }
    private void OnDisable()
    {
        EnemyHandler.OnEnemiesDefeated -= EnemyStateReset;
        PlayerStats.OnDamagePlayer -= PlayerDamaged;
        PlayerStats.OnDeathPlayer -= PlayerDeath;
    }

    /* resets "CombatState.CombatStart" variable when all enemies are dead (variable is set to false by default, and set true upon completing dialogue).
    * resets "CombatState.CombatActive" variable when all enemies are dead. Allows for bark dialogue to play only when in combat.
    * Allows for restarting the same car without retriggering dialogue in cases of death or checkpoints, so long as OnEnemiesDefeated is not called.
    * Manually set what starting convo to have, if any, in DialogueTrigger/StartConversationTrigger prefab. Attach prefab to all scenes that have opening dialogue.
    */
    private void EnemyStateReset()
    {
        DialogueLua.SetVariable("CombatState.CombatStart", false);
        DialogueLua.SetVariable("CombatState.CombatActive", false);
     }

    /* Dynamically adjusts dialogue variables for varied enemy bark lines (NOT IMPLEMENTED YET). Checks if player is active, triggers death cutscene if not.
     * Death cutscene 
     */
    private void PlayerDamaged(float currentDamage)
    {
        //not currently implemented. Will alter dialogue based on players current HP.
    }
    private void PlayerDeath() 
    {
        int quip = Random.Range(0, 3);
        DialogueLua.SetVariable("DeathQuip", quip);

        playerDeathTrigger.OnUse();
        //ToDo: Also call whatever method resets the scene.
    }
    private void StartConvo()
    {
        int taunt = Random.Range(0, 7);
        DialogueLua.SetVariable("GenericIntro", taunt);
        startTrigger.OnUse();
    }
}