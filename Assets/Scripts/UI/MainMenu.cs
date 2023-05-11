using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject mainMenuScreen;

    // Start is called before the first frame update
    void Start()
    {
        OpenMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCredits()
    {
        creditsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void OpenMainMenu()
    {
        creditsScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void ExitGame()
    {

    }


}
