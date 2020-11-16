using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Selection : MonoBehaviour {
   
   public static int choice = 0;

   public void playGameVsCharizard() {
        choice = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
    public void playGameVsPikachu() {
        choice = 2;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void playGameVsVenusaur() {
        choice = 3;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }


}
