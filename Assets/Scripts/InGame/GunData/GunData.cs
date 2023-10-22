using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    [SerializeField]
    private int _ammo;//ˆê‚Â‚Ìƒ}ƒKƒWƒ“‚É“ü‚Á‚Ä‚¢‚é’e‚Ì”
    [SerializeField]
    private int _damage;//ˆê”­‚Ìƒ_ƒ[ƒW
    [SerializeField]
    private float _fireRate;//‚P•ªŠÔ‚É”­ŽË‚·‚é’e‚Ì”
    [SerializeField]
    private float _reloadTime;//ƒŠƒ[ƒhŽžŠÔ

    public int ammo { get => _ammo; set => _ammo = value; }
    public int damage { get => _damage; set => _damage = value; }
    public float fireRate { get => _fireRate; set => _fireRate = value; }
    public float reloadTime { get => _reloadTime; set => _reloadTime = value; }
}
