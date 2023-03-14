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

    public void FinishPuzzle()
    {
        switch (currentPuzzleType)
        {
            case PuzzleType.Generic:
                break;
            case PuzzleType.Door:
                gameObject.GetComponent<ItemAnimationController>().enabled = true;
                break;
            default:
                break;
        }
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
