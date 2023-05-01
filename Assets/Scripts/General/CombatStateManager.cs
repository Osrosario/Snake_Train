using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateManager : MonoBehaviour
{
    [SerializeField] SceneState initialState;
    [SerializeField] private int enemyCounter;

    public static Action<int> SendSceneState;

    public static CombatStateManager current;

    private SceneState _currentSceneState;

    public enum SceneState
    {
        Friendly,
        Neutral,
        Hostile,
        Transition
    }

    /*
     * Subscribes to the OnCreate event in the EnemyStats script.
     * Subscribes to the OnDestroy event in the EnemyStats script.
     * Invokes: IncreaseEnemyCount() and DecreaseEnemyCount()
     */
    private void OnEnable()
    {
        EnemyStats.OnSpawn += IncreaseEnemyCount;
        EnemyStats.OnDestroy += DecreaseEnemyCount;
    }

    /*
    * Unsubscribes from the OnCreate event in the EnemyStats script (if destroyed).
    * Unsubscribes from the OnDestroy event in the EnemyStats script (if destroyed).
    */
    private void OnDisable()
    {
        EnemyStats.OnSpawn -= IncreaseEnemyCount;
        EnemyStats.OnDestroy -= DecreaseEnemyCount;
    }

    private void Start()
    {
        current = this;
        _currentSceneState = initialState;
        SetState(_currentSceneState);
    }

    /*
     * Combat Finite State Machine.
     * Calls all functions subscribed to this event.
     * Subscription: TransitionController, TransitionPrompter, GatlingStateManager, PlayerMovement, PlayerAttack.
     */
    private void Update()
    {
        switch (_currentSceneState)
        {
            case SceneState.Friendly:

                SendSceneState?.Invoke(0);
                break;

            case SceneState.Neutral:

                SendSceneState?.Invoke(1);
                break;

            case SceneState.Hostile:

                SendSceneState?.Invoke(2);
                break;

            case SceneState.Transition:

                SendSceneState?.Invoke(3);
                break;
        }
    }

    /* Changes Scene State. */
    public void SetState(SceneState state)
    {
        _currentSceneState = state;
    }

    private void IncreaseEnemyCount() 
    { 
        enemyCounter++; 
    }

    private void DecreaseEnemyCount() 
    { 
        enemyCounter--;

        if (enemyCounter <= 0)
        {
            _currentSceneState = initialState;
            SetState(_currentSceneState);
        }
    }

    /* Getter/Setter */
    public SceneState CurrentSceneState
    {
        get { return _currentSceneState; }
        set { _currentSceneState = value; }
    }
}
