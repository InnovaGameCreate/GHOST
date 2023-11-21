using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using InGame.Player;
using UniRx;

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
    [SerializeField]
    GameObject cameraObject;
    [SerializeField]
    private PlayerStatus _playerStatus;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    PistolSounds _sound;
    [Networked] private TickTimer callTime { get; set; }
    RaycastHit hit;
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

        _playerStatus.useEMP
            .Where(value => value)
            .Subscribe(_ =>
            {
                _currentAmmo--;
                if(_currentAmmo <= 0)
                {
                    _playerStatus.AddDamage(100);
                }
                Debug.Log("最大段数を一つ減少");
            }).AddTo(this);
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
                if (_currentAmmo > 0)
                {
                    InstanceBulletPrefab();
                    _sound.Shoot();
                    HitCheck();
                    _currentAmmo--;
                    _networkFPSController.Fire();
                    callTime = TickTimer.CreateFromSeconds(Runner, 60 / fireRate);
                }
                else
                {
                    _playerStatus.AddDamage(100);
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
    private void HitCheck()
    {
        GameObject originObject = cameraObject;
        Vector3 originPositon = originObject.transform.position;
        Vector3 direction = originObject.transform.forward;
        Ray ray = new Ray(originPositon, direction);
        Debug.DrawRay(originPositon, direction, Color.cyan, 3);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.TryGetComponent(out NetworkObject nerwork))
            {
                if (!nerwork.HasStateAuthority)
                {
                    if (hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.AddDamage(damage);
                        if(damageable.GetCurrentHp() <= 0)
                        {
                            _currentAmmo = _gunData.ammo;
                        }
                    }
                }
            }
        }
    }
}
