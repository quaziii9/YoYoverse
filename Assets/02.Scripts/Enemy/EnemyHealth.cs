using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamage
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

    public void TakeDamage(float damage)
    {
        enemyHealth -= (int)damage; //이거 당장은 형변환으로 막았는데 피통 int 사용할거면 이렇게 쓰고 아니면 피통 float으로 변환해주셈.

        if(enemyHealth <= 0)
        {
            //여기안에 사망처리 하면 어떻게 동작할지 구현 ㄱㄱ
        }
    }

    
}
