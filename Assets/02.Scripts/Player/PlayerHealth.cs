using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float _health;

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if(_health <= 0)
        {
            DebugLogger.Log("사망");
        }
    }
}
