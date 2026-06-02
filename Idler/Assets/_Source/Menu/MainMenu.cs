using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button settings;
    [SerializeField] private Button exit;
    [SerializeField] private GameObject settingPanel;

    private void Start()
    {
        play.onClick.AddListener(Play);
        settings.onClick.AddListener(Settings);
        exit.onClick.AddListener(Exit);
    }

    private void Play()
    {
        SceneManager.LoadScene(0);
    }

    private void Settings()
    {
        settingPanel.SetActive(true);
    }

    private void Exit()
    {
        Application.Quit();
    }
}
