using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
public class Bullet : MonoBehaviour
{
    public GameObject shooter; // 총알을 발사한 객체
    private float DestroyBulletTime = 1f;
    private float _power = 1;

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
                hit.TakeDamage(_power);
                gameObject.SetActive(false);
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

    void ProjectileDisable(Vector3 hitPosition)
    {
        //EffectManager.Instance.HitEffectGenenate(hitPosition);
        ObjectPool.Instance.EnqueueObject(gameObject);
    }

    IEnumerator DisableBullet()
    {
        yield return new WaitForSecondsRealtime(DestroyBulletTime);
        gameObject.SetActive(false);
    }
}