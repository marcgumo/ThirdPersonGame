using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public enum CharacterType { Player, Enemy01 }

    [Header("Health settings")]
    public CharacterType currentCharacterType = CharacterType.Player;
    public bool friendlyFire = false;

    [SerializeField] private Image hpBar;

    [Header("Stats settings")]
    [SerializeField] private int maxHealth;
    int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
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
                //Restart game, Game over
                return;
            }

            if (currentCharacterType == CharacterType.Enemy01)
            {
                Destroy(gameObject);
            }
        }
    }

    public void HPBarUpdate()
    {
        hpBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
