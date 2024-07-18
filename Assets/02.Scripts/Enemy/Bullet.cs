using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
public class Bullet : MonoBehaviour
{
    public GameObject shooter; // 총알을 발사한 객체
    public float DestroyBulletTime;
    public float _attackPower = 1.0f;

    private void OnEnable()
    {
        StartCoroutine(DisableBullet());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == shooter)
        {
            // 발사한 적 자신과의 충돌은 무시
            return;
        }

        if (other.CompareTag("Player"))
        {
            IDamage hit = other.gameObject.GetComponent<IDamage>();

            if (hit != null)
            {
                hit.TakeDamage(_attackPower);
                ProjectileDisable(transform.position);
            }
            Debug.Log("enter player");
        }
        else
            Debug.Log(other.name);

       
    }

    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
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