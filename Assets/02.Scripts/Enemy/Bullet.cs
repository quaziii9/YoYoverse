using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject shooter; // 총알을 발사한 객체
    private float DestroyBulletTime = 1f;
    private float _power = 1;
    public GameObject bulletHitEffect; // Add this line

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DisableBullet());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            IDamage hit = other.gameObject.GetComponent<IDamage>();
            Player player = other.gameObject.GetComponent<Player>();
            PlayerStateMachine playerStateMachine = player.PlayerStateMachine;

            if (hit != null && playerStateMachine._currentState !=
                                playerStateMachine._stateDictionary[State.SkillDefense])
            {
                GameObject hitParticle = ObjectPool.Instance.DequeueObject(bulletHitEffect);
                hitParticle.transform.position = other.transform.position + new Vector3(0, 1.5f, 0);
                ParticleSystem particleSystem = hitParticle.GetComponent<ParticleSystem>();
                particleSystem.Play();
                hit.TakeDamage(_power);
                ObjectPool.Instance.EnqueueObject(gameObject);
                Debug.Log("hit");
            }
        }
    }

    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
    }

    public void SetBulletLifeTime(float destroybulletTime)
    {
        DestroyBulletTime = destroybulletTime;
    }

    void ProjectileDisable()
    {
        ObjectPool.Instance.EnqueueObject(gameObject);
    }

    IEnumerator DisableBullet()
    {
        yield return new WaitForSecondsRealtime(DestroyBulletTime);
        gameObject.SetActive(false);
    }

    private void HitEffectGenerate(Vector3 position)
    {
        GameObject item = ObjectPool.Instance.DequeueObject(bulletHitEffect);
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;
        StartCoroutine(EnqueueObject(item, 0.2f));
    }

    private IEnumerator EnqueueObject(GameObject item, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPool.Instance.EnqueueObject(item);
    }
}