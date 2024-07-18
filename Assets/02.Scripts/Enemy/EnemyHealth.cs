using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealth;
    public int currentEnemyHealth;

    public void Start()
    {
        currentEnemyHealth = enemyHealth;
    }

    public void EnemyGetDamage(int damage)
    {
        currentEnemyHealth -= damage;
    }

    public void BeAssassinateDamage()
    {
        currentEnemyHealth = 0;
    }
}
