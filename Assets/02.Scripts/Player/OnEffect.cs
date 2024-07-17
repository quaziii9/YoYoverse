using EnumTypes;
using EventLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnEffect : MonoBehaviour
{
    [Header("Key")]
    [SerializeField] private int _key;

    [Header("HitEffect")]
    [SerializeField] private GameObject _hitEffect;

    private BoxCollider _hitCollider;
    private float _power;
    private float _offTime = 0.5f;
    private bool canHit = true;

    private void Awake()
    {
        EventManager<PlayerEvents>.StartListening<int>(PlayerEvents.OnAttackEffect, OnHitCollider);
        _hitCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        if (_hitEffect == null)
            return;

        ObjectPool.Instance.CreatePool(_hitEffect, 20);
    }

    public void SetPower(float power)
    {
        _power = power;
    }

    public void TriggerEvent(int key)
    {
        EventManager<PlayerEvents>.TriggerEvent<int>(PlayerEvents.OnAttackEffect, key);
    }

    private void OnHitCollider(int key)
    {
        if(_key == key)
        {
            _hitCollider.enabled = true;

            Invoke(nameof(OffHitCollider), _offTime);
        }
    }

    private void OffHitCollider()
    {
        _hitCollider.enabled = false;
    }

    private void ReturnHitEffect(GameObject hitEffect)
    {
        if (hitEffect == null)
            return;

        ObjectPool.Instance.EnqueueObject(hitEffect);
    }

    //오버랩 스피어를 발생시켜서 충돌을 검사하는 메소드. (충돌이 4번씩 판정되서 일단 보류시킴.)
    private void Hit(Collider other) 
    {
        Collider[] colliders = Physics.OverlapSphere(other.transform.position, 0.1f);

        foreach(Collider target in colliders)
        {
            if(target.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                IDamage hit = target.GetComponent<IDamage>();

                if (hit != null)
                {
                    GameObject hitEffect = ObjectPool.Instance.DequeueObject(_hitEffect);

                    hitEffect.transform.position = other.transform.position;

                    ReturnHitEffect(hitEffect);

                    hit.TakeDamage(_power);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Hit(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && canHit)
        {
            canHit = false;

            IDamage hit = other.gameObject.GetComponent<IDamage>();

            if (hit != null)
            {
                GameObject hitEffect = ObjectPool.Instance.DequeueObject(_hitEffect);

                hitEffect.transform.position = other.transform.position;

                ReturnHitEffect(hitEffect);

                hit.TakeDamage(_power);

                canHit = true;
            }
        }
    }
}
