using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("HUD panel settings")]
    [SerializeField] private TextMeshProUGUI coinsTextCounter;

    int totalCoins = 0;
    int startCoins;

    public static Action OnUpdateCoins;

    private void Start()
    {
        startCoins = GameObject.FindGameObjectWithTag("CoinsList").transform.childCount;
    }

    public void UpdateTotalCoins()
    {
        totalCoins++;
        coinsTextCounter.text = totalCoins.ToString();

        if (totalCoins == startCoins)
            OnUpdateCoins?.Invoke();
    }
}
