using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;

public class PlayerRPCManager : NetworkBehaviour
{
    [SerializeField]
    PlayerStatus _playerStatus;
    void Start()
    {
        _playerStatus.currentHp
            .Subscribe(hp => {
                RPC_UpdateCurrentHp(hp);
            }).AddTo(this);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateCurrentHp(float _updateHp, RpcInfo info = default)
    {
        if (!info.IsInvokeLocal)
        {
            Debug.Log("Invoke:Local");
            _playerStatus.setHp(_updateHp);
            Debug.Log($"RPC_ダメージをうけた！体力が{_updateHp}になった");
        }
        else
        {
            Debug.Log("Invoke:notLocal");
        }
    }
}
