using UnityEngine;
using Fusion;
using UniRx;
using System;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _baseHp;
    private ReactiveProperty<float> _currentHp = new ReactiveProperty<float>(100);
    private ReactiveProperty<bool> _useEMP = new ReactiveProperty<bool>();
    private bool _isLocalPlayer = false;
    public IObservable<float> currentHp//_currentHp�̕ω����������Ƃ��ɔ��s�����
    {
        get { return _currentHp; }
    }

    public IObservable<bool> useEMP//_currentHp�̕ω����������Ƃ��ɔ��s�����
    {
        get { return _useEMP; }
    }

    public float maxBaseHp { get => _baseHp; }
    public bool isLocalPlayer { get => _isLocalPlayer; }

    public void AddDamage(float damage)
    {
        _currentHp.Value -= damage;
        Debug.Log($"�_���[�W���������I�̗͂�{_currentHp}�ɂȂ���");
        if (_currentHp.Value < 0)
            _currentHp.Value = 0;
    }

    void Awake()
    {
        _currentHp.Value = _baseHp;
    }

    public void setLocalCharacter()
    {
        _isLocalPlayer = true;
    }

    public void setHp(float hp)
    {
        _currentHp.Value = hp;
    }

    public void UseEMP(bool useValue)
    {
        _useEMP.Value = useValue;
        Debug.Log($"UseEMP{useValue}");
    }

}
