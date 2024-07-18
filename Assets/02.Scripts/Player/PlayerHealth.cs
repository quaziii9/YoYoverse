using System.Collections;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float _health;

    private Animator _animator;
    private static readonly int DieParam = Animator.StringToHash("Die");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if(_health <= 0)
        {
            StartCoroutine(Die());
        }
        
        EventManager<PlayerEvents>.TriggerEvent<float>(PlayerEvents.PlayerDamaged, damage);
    }

    private IEnumerator Die()
    {
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        EventManager<GameEvents>.TriggerEvent(GameEvents.PlayerDeath); // 플레이어 사망 이벤트 발생
        _animator.SetTrigger(DieParam);

        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }
}
