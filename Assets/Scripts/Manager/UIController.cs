using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("HUD panel settings")]
    [SerializeField] private TextMeshProUGUI coinsTextCounter;

    int totalCoins = 0;

    public void UpdateTotalCoins()
    {
        totalCoins++;
        coinsTextCounter.text = totalCoins.ToString();
    }
}
