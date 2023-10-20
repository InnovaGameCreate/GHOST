using UnityEngine;
using Fusion;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _baseHp;
    private float _currentHp;

    public void AddDamage(float damage)
    {
        _currentHp -= damage;
        if (_currentHp < 0)
            _currentHp = 0;
    }

    void Start()
    {
        _currentHp = _baseHp;
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void UpdateCurrentHp(float _updateHp)
    {
        if(_currentHp != _updateHp)
        {
            _currentHp = _updateHp;
        }
    }

}
