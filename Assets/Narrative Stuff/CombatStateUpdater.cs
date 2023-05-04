using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatStateUpdater : MonoBehaviour
{
    public void StateActive()
    {
        Invoke("StateActiveSet", 2f);
    }
    public void StateIdle()
    {
        CombatStateManager.current.SetState(CombatStateManager.SceneState.Friendly);
    }

    private void StateActiveSet()
    {
        CombatStateManager.current.SetState(CombatStateManager.SceneState.Hostile);
    }
    public void GoToLoad()
    {
        Invoke("LoadScene", 2f);
    }
    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
