using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    [SerializeField]
    private int _ammo;//��̃}�K�W���ɓ����Ă���e�̐�
    [SerializeField]
    private int _damage;//�ꔭ�̃_���[�W
    [SerializeField]
    private float _fireRate;//�P���Ԃɔ��˂���e�̐�
    [SerializeField]
    private float _reloadTime;//�����[�h����

    public int ammo { get => _ammo; set => _ammo = value; }
    public int damage { get => _damage; set => _damage = value; }
    public float fireRate { get => _fireRate; set => _fireRate = value; }
    public float reloadTime { get => _reloadTime; set => _reloadTime = value; }
}
