using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GunBase : NetworkBehaviour
{
    [SerializeField]
    private GunData _gunData;
    private int ammo;//一つのマガジンに入っている弾の数
    private int damage;//一発のダメージ
    private float fireRate;//１分間に発射する弾の数
    private float reloadTime;//リロード時間
    private bool _isFire;//銃を打っているかどうか
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
            Debug.Log($"残り残段数{currentAmmo}");
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
            Debug.LogError("BulletPrefabにBullet.csがアタッチされていません");
        }
    }
}
