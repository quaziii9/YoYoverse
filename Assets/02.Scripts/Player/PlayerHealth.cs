using EnumTypes;
using EventLibrary;
using System.Collections;
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
            EventManager<GameEvents>.TriggerEvent(GameEvents.PlayerDeath); // 플레이어 사망 이벤트 발생
        }
        
        EventManager<PlayerEvents>.TriggerEvent<float>(PlayerEvents.PlayerDamaged, damage);
    }

    public float GetHealth()
    {
        return _health;
    }
}
