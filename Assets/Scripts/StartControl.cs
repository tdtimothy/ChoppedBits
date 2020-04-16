using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartControl : MonoBehaviour
{
   public GameObject StartMenu;
   public GameObject tutorial;
   public static bool Endless;
   public void StoryMode() {
      StartMenu.SetActive(false);
      startTutorial();
   }

   void startTutorial(){
      tutorial.SetActive(true);
   }

   public void EndlessMode() {
      Endless = true;
      StartCoroutine(LevelOneWait());
   }

   IEnumerator LevelOneWait()
   {
      yield return new WaitForSeconds(1);
      SceneManager.LoadScene("Puzzle0-1", LoadSceneMode.Single);
   }
}
