using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GunBase : NetworkBehaviour
{
    [SerializeField]
    private GunData _gunData;
    private int ammo;//��̃}�K�W���ɓ����Ă���e�̐�
    private int damage;//�ꔭ�̃_���[�W
    private float fireRate;//�P���Ԃɔ��˂���e�̐�
    private float reloadTime;//�����[�h����
    private bool _isFire;//�e��ł��Ă��邩�ǂ���
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float bulletSpeed;
    void Start()
    {
        if (!HasStateAuthority) return;
        ammo = _gunData.ammo;
        damage = _gunData.damage;
        fireRate = _gunData.fireRate;
        reloadTime = _gunData.reloadTime;
        StartCoroutine(FireControll());
    }
    public void OpenFire()
    {
        _isFire = true;
    }
    public void StopFire()
    {
        _isFire = false;
    }

    IEnumerator FireControll()
    {
        int currentAmmo = ammo;
        while (true)
        {
            yield return new WaitUntil(() => _isFire);
            InstanceBulletPrefab();
            currentAmmo--;
            Debug.Log($"�c��c�i��{currentAmmo}");
            if (currentAmmo > 0)
                yield return new WaitForSeconds(60 / fireRate);
            else
            {
                yield return new WaitForSeconds(reloadTime);
                currentAmmo = ammo;
            }

        }
    }

    private void InstanceBulletPrefab()
    {
        var Bullet = Runner.Spawn(_bulletPrefab, transform.position, Quaternion.identity, Runner.LocalPlayer);
        if(Bullet.TryGetComponent(out Bullet bulletCs))
        {
            bulletCs.dataSet(damage, bulletSpeed);
            Bullet.GetComponent<Rigidbody>().AddForce(transform.root.forward * bulletSpeed, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("BulletPrefab��Bullet.cs���A�^�b�`����Ă��܂���");
        }
    }
}
