using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    private static string sceneName;
    static MusicPlayer _instance;
    void Awake()
    {
        if(sceneName == null)
            sceneName = SceneManager.GetActiveScene().name;
        else if(sceneName != SceneManager.GetActiveScene().name) {
            _instance = this;
            sceneName = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (_instance == null) {    
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(_instance != this)
            Destroy(this.gameObject);
    }
    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
         
    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(_instance != this)
            Destroy(this.gameObject);
    }
}
