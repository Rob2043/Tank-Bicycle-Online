using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text bestScoreText;
    private void Start() {
        bestScoreText.text = $"{PlayerPrefs.GetInt("BestScore",0)}";
    }
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Settings()
    {
        
    }

    public void Online()
    {
        
    }
}
