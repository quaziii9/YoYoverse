using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisableBullet());
    }

    private void OnCollisionEnter(Collision col)
    {
        Collider collider = col.collider;

        if (collider.CompareTag("Player"))
            Debug.Log("enter player");
        else
            Debug.Log("else ");
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