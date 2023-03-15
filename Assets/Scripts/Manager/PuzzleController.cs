using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public enum PuzzleType
    {
        Generic, Door
    }

    [SerializeField] private PuzzleType currentPuzzleType;

    [Header("Door puzzle settings")]
    [SerializeField] private CinemachineFreeLook playerCamera;
    [SerializeField] private CinemachineVirtualCamera doorPuzzleCamera;

    UIController UIController;

    private void Start()
    {
        UIController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
    }

    public void FinishPuzzle()
    {
        switch (currentPuzzleType)
        {
            case PuzzleType.Generic:
                break;
            case PuzzleType.Door:

                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).GetComponent<ItemAnimationController>() != null)
                        transform.parent.GetChild(i).GetComponent<ItemAnimationController>().enabled = true;
                }

                StartCoroutine(ResetCameraPriority(playerCamera.m_XAxis.m_MaxSpeed, playerCamera.m_YAxis.m_MaxSpeed));

                playerCamera.Priority = 0;
                doorPuzzleCamera.Priority = 1;

                playerCamera.m_XAxis.m_MaxSpeed = 0;
                playerCamera.m_YAxis.m_MaxSpeed = 0;

                Time.timeScale = 0;
                UIController.GameIsPaused = true;
                break;
            default:
                break;
        }
    }

    IEnumerator ResetCameraPriority(float xAxis, float yAxis)
    {
        yield return new WaitForSecondsRealtime(3);
        playerCamera.Priority = 1;
        doorPuzzleCamera.Priority = 0;

        yield return new WaitForSecondsRealtime(1);
        playerCamera.m_XAxis.m_MaxSpeed = xAxis;
        playerCamera.m_YAxis.m_MaxSpeed = yAxis;

        Time.timeScale = 1;

        StartCoroutine(ResumeGameLater());
    }

    IEnumerator ResumeGameLater()
    {
        yield return null;

        UIController.GameIsPaused = false;
    }

    private void OnEnable()
    {
        UIController.OnUpdateCoins += FinishPuzzle;
    }

    private void OnDisable()
    {
        UIController.OnUpdateCoins -= FinishPuzzle;
    }
}
