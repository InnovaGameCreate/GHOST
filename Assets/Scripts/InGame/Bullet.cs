using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    private float damage;
    private float bulletSpeed;
    private NetworkObject _networkObject;
    [Networked] private TickTimer _breakTime { get; set; }

    private void Start()
    {
        if (!HasStateAuthority) return;
        _networkObject = GetComponent<NetworkObject>();
        _breakTime = TickTimer.CreateFromSeconds(Runner,5f);
    }


    public void dataSet(float setDamage, float setBulletSpeed)
    {
        damage = setDamage;
        bulletSpeed = setBulletSpeed;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
       // transform.position += transform.forward * Runner.DeltaTime * bulletSpeed;
        if (_breakTime.Expired(Runner))
        {
            Runner.Despawn(_networkObject);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!HasStateAuthority) return;
        if (coll.CompareTag("Player"))
        {
            if (coll.TryGetComponent(out NetworkObject nerwork))
            {
                if (!nerwork.HasStateAuthority)
                {
                    if (coll.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.AddDamage(damage);
                    }
                }                
            }
        }
    }
}
