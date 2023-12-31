using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    [SerializeField]
    private int _ammo;//一つのマガジンに入っている弾の数
    [SerializeField]
    private int _damage;//一発のダメージ
    [SerializeField]
    private float _fireRate;//１分間に発射する弾の数
    [SerializeField]
    private float _reloadTime;//リロード時間

    public int ammo { get => _ammo; set => _ammo = value; }
    public int damage { get => _damage; set => _damage = value; }
    public float fireRate { get => _fireRate; set => _fireRate = value; }
    public float reloadTime { get => _reloadTime; set => _reloadTime = value; }
}
