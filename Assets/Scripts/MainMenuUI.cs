using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        //this is lambda function from delegates
        Time.timeScale = 1f;
    }
    
   
}
