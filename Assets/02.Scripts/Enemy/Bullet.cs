using System.Collections;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public GameObject shooter; // 총알을 발사한 객체

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
            Debug.Log("enter player");
        else
            Debug.Log(other.name);

       // ProjectileDisable(transform.position);
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
        yield return new WaitForSecondsRealtime(2f);
        gameObject.SetActive(false);
    }
}