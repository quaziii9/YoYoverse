using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinWeapon : MonoBehaviour
{
    private float _power;
    private bool canHit = true;
    private BoxCollider _hitCollider;

    void Start()
    {
        _hitCollider = GetComponent<BoxCollider>();
    }

    public void SetPower(float power)
    {
        _power = power;
    }

    public void OnHitCollider()
    {
        _hitCollider.enabled = true;

        canHit = true;

        Invoke(nameof(OffCollider), 1.0f);
    }

    private void OffCollider()
    {
        _hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && canHit)
        {
            DebugLogger.Log("Paladin Attack");
            
            OffCollider();

            canHit = false;

            IDamage hit = other.gameObject.GetComponent<IDamage>();

            if(hit != null)
            {
                hit.TakeDamage(_power);
            }
        }
    }
}
