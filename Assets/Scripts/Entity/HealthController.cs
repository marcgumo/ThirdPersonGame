using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public enum CharacterType { Player, Enemy01 }

    [Header("Health settings")]
    public CharacterType currentCharacterType = CharacterType.Player;
    public bool friendlyFire = false;

    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI lifesTextCounter;
    [SerializeField] private TextMeshProUGUI EnemiesTextCounter;

    [Header("Stats settings")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int maxLifes;
    int currentHealth;
    int currentLifes;
    public int CurrentEnemies { get; set; }

    public static Action<GameObject> onEnemyDead;
    public static Action onPlayerDead;
    public static Action onGameOver;

    void Start()
    {
        currentLifes = maxLifes;
        currentHealth = maxHealth;

        if (lifesTextCounter) LifesUpdate();
        if (hpBar) HPBarUpdate();
    }

    public void TakeDamage(int damage, string tag)
    {
        if (gameObject.tag != tag && !friendlyFire)
            return;

        currentHealth -= damage;

        if (hpBar) HPBarUpdate();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (currentCharacterType == CharacterType.Player)
            {
                currentLifes--;
                if (lifesTextCounter) LifesUpdate();


                if (currentLifes <= 0)
                {
                    currentLifes = 0;
                    if (lifesTextCounter) LifesUpdate();

                    onGameOver?.Invoke();
                }
                else
                {
                    onPlayerDead?.Invoke();

                    currentHealth = maxHealth;
                    if (hpBar) HPBarUpdate();
                }

                return;
            }

            if (currentCharacterType == CharacterType.Enemy01)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>().CurrentEnemies++;
                if (EnemiesTextCounter) EnemiesUpdate();

                onEnemyDead?.Invoke(gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void EnemiesUpdate()
    {
        EnemiesTextCounter.text = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>().CurrentEnemies.ToString();
    }

    private void LifesUpdate()
    {
        lifesTextCounter.text = currentLifes.ToString();
    }

    public void HPBarUpdate()
    {
        hpBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
