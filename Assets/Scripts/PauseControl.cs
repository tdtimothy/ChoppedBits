using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject pauseMenuUI;
 
     // Update is called once per frame
    void Update()
    {
        if(!GameController.gameActive)
            return;
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }
 
    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame() {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}