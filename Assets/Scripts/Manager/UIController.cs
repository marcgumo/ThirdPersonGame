using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public enum Menus
    {
        MainMenu, GameOverMenu, PauseMenu, LevelCompletedMenu
    }

    [Header("HUD panel settings")]
    [SerializeField] private TextMeshProUGUI coinsTextCounter;
    [SerializeField] private TextMeshProUGUI puzzlesTextCounter;

    int totalCoins = 0;
    int startCoins;

    int totalPuzzles;

    public static Action OnUpdateCoins;
    public static Action OnUpdatePuzzles;

    public bool GameIsPaused { get; set; }

    [Header("Quiz panel settings")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private CinemachineFreeLook playerCamera;
    [SerializeField] private UnityEvent quizAction;

    public bool quizReady { get; set; }
    public bool quizOpen { get; set; }

    [Header("Menu panels settings")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelCompletedPanel;

    private float[] playerCameraValues = new float[2];

    private void Start()
    {
        startCoins = GameObject.FindGameObjectWithTag("CoinsList").transform.childCount;

        StopGame();
        ShowHideMenu((int)Menus.MainMenu);
    }

    private void Update()
    {
        if (quizReady && !quizOpen && Input.GetKeyDown(KeyCode.E))
        {
            playerCameraValues[0] = playerCamera.m_XAxis.m_MaxSpeed;
            playerCameraValues[1] = playerCamera.m_YAxis.m_MaxSpeed;

            StartGameQuiz();

            StopGame();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !GameIsPaused)
        {
            ShowHideMenu((int)Menus.PauseMenu);
            StopGame();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ShowHideMenu((int)Menus.PauseMenu);
            ResumeGame();
        }
    }

    public void ShowHideMenu(int value)
    {
        switch ((Menus)value)
        {
            case Menus.MainMenu:
                mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
                break;
            case Menus.GameOverMenu:
                gameOverPanel.SetActive(!gameOverPanel.activeSelf);
                break;
            case Menus.PauseMenu:
                pausePanel.SetActive(!pausePanel.activeSelf);
                break;
            case Menus.LevelCompletedMenu:
                levelCompletedPanel.SetActive(!levelCompletedPanel.activeSelf);
                break;
            default:
                break;
        }
    }

    public void TextToDisplay(bool value)
    {
        if (!quizOpen)
        {
            quizReady = value;
            textPanel.SetActive(value);
        }
    }

    private void ResumeGameQuiz()
    {
        quizOpen = false;
        TextToDisplay(true);
    }

    private void StartGameQuiz()
    {
        TextToDisplay(false);

        quizOpen = true;
        quizAction.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        StartCoroutine(ResumeGameLater());

        Cursor.lockState = CursorLockMode.Locked;

        playerCamera.m_XAxis.m_MaxSpeed = playerCameraValues[0];
        playerCamera.m_YAxis.m_MaxSpeed = playerCameraValues[1];
    }

    IEnumerator ResumeGameLater()
    {
        yield return null;
        GameIsPaused = false;
    }

    public void StopGame()
    {
        Time.timeScale = 0;

        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;

        playerCamera.m_XAxis.m_MaxSpeed = 0;
        playerCamera.m_YAxis.m_MaxSpeed = 0;
    }

    public void GetAnswer(int value)
    {
        switch (value)
        {
            case 0:
                Debug.Log("Incorrect Answer");
                ResumeGame();
                ResumeGameQuiz();
                break;
            case 1:
                Debug.Log("Correct Answer");
                ResumeGame();
                ResumeGameQuiz();
                break;
            case 2:
                Debug.Log("Incorrect Answer");
                ResumeGame();
                ResumeGameQuiz();
                break;
        }
    }

    public void UpdateTotalCoins()
    {
        totalCoins++;
        coinsTextCounter.text = totalCoins.ToString();

        if (totalCoins == startCoins)
            OnUpdateCoins?.Invoke();
    }

    public void UpdateTotalPuzzles()
    {
        totalPuzzles++;
        puzzlesTextCounter.text = totalPuzzles.ToString();

        if (totalPuzzles == 3)
        {
            OnUpdatePuzzles?.Invoke();
        }
    }
}
