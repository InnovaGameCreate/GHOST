using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using InGame.Player;

public class GunBase : NetworkBehaviour
{
    [SerializeField]
    private GunData _gunData;
    private int ammo;//一つのマガジンに入っている弾の数
    private int damage;//一発のダメージ
    private float fireRate;//１分間に発射する弾の数
    private float reloadTime;//リロード時間
    private bool _isFire;//銃を打っているかどうか
    private int _currentAmmo;//現在の弾の数
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float bulletSpeed;
    private PlayerFire _playerFire;
    [SerializeField]
    NetworkFPSController _networkFPSController;
    [Networked] private TickTimer callTime { get; set; }
    void Start()
    {
        if (!HasStateAuthority) return;
        ammo = _gunData.ammo;
        damage = _gunData.damage;
        fireRate = _gunData.fireRate;
        reloadTime = _gunData.reloadTime;

        _playerFire = gameObject.transform.root.GetComponent<PlayerFire>();
        _playerFire.setGunBase(this);
        _currentAmmo = ammo;
    }
    public void OpenFire()
    {
        _isFire = true;
    }
    public void StopFire()
    {
        _isFire = false;
        _networkFPSController.OnFireReleased();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            if(_isFire && callTime.ExpiredOrNotRunning(Runner))
            {
                InstanceBulletPrefab();
                _currentAmmo--;
                if (_currentAmmo > 0)
                {
                    _networkFPSController.Fire();
                    callTime = TickTimer.CreateFromSeconds(Runner, 60 / fireRate);
                }
                else
                {
                    _networkFPSController.OnFireReleased();
                    callTime = TickTimer.CreateFromSeconds(Runner, reloadTime);
                    _currentAmmo = ammo;
                }
            }
        }
    }

    private void InstanceBulletPrefab()
    {
        Debug.Log("InstanceBulletPrefab");
        var Bullet = Runner.Spawn(_bulletPrefab, transform.position, Quaternion.identity, Runner.LocalPlayer);
        if(Bullet.TryGetComponent(out Bullet bulletCs))
        {
            bulletCs.dataSet(damage, bulletSpeed);
            Bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("BulletPrefabにBullet.csがアタッチされていません");
        }
    }
}
