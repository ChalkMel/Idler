using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button play;
    [SerializeField] private Button settings;
    [SerializeField] private Button save;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private Button exit;
    [SerializeField] private GameObject settingPanel;

    private void Start()
    {
        play.onClick.AddListener(Play);
        settings.onClick.AddListener(Settings);
        exit.onClick.AddListener(Exit);
        save.onClick.AddListener(SavePanel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }


    private void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }
    
    private void Play()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    private void Settings()
    {
        settingPanel.SetActive(true);
    }

    private void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    private void SavePanel()
    {
        savePanel.SetActive(true);
    }
}
