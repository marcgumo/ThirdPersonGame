using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [Header("HUD panel settings")]
    [SerializeField] private TextMeshProUGUI coinsTextCounter;

    int totalCoins = 0;
    int startCoins;

    public static Action OnUpdateCoins;

    public bool gameIsPaused { get; set; }

    [Header("Quiz panel settings")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private CinemachineFreeLook playerCamera;
    [SerializeField] private UnityEvent quizAction;

    public bool quizReady { get; set; }
    public bool quizOpen { get; set; }

    private float[] playerCameraValues = new float[2];

    private void Start()
    {
        startCoins = GameObject.FindGameObjectWithTag("CoinsList").transform.childCount;
    }

    private void Update()
    {
        if (quizReady && !quizOpen && Input.GetKeyDown(KeyCode.E))
        {
            gameIsPaused = true;
            Time.timeScale = 0;

            TextToDisplay(false);

            quizOpen = true;
            quizAction.Invoke();

            Cursor.lockState = CursorLockMode.None;

            playerCameraValues[0] = playerCamera.m_XAxis.m_MaxSpeed;
            playerCameraValues[1] = playerCamera.m_YAxis.m_MaxSpeed;

            playerCamera.m_XAxis.m_MaxSpeed = 0;
            playerCamera.m_YAxis.m_MaxSpeed = 0;
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

    public void ResumeGame()
    {
        Time.timeScale = 1;

        StartCoroutine(ResumeGameLater());

        Cursor.lockState = CursorLockMode.Locked;

        quizOpen = false;
        TextToDisplay(true);

        playerCamera.m_XAxis.m_MaxSpeed = playerCameraValues[0];
        playerCamera.m_YAxis.m_MaxSpeed = playerCameraValues[1];
    }

    IEnumerator ResumeGameLater()
    {
        yield return null;
        gameIsPaused = false;
    }

    public void GetAnswer(int value)
    {
        switch (value)
        {
            case 0:
                Debug.Log("Incorrect Answer");
                ResumeGame();
                break;
            case 1:
                Debug.Log("Correct Answer");
                ResumeGame();
                break;
            case 2:
                Debug.Log("Incorrect Answer");
                ResumeGame();
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
}
