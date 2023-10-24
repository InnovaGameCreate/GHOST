using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;

public class PlayerRPCManager : NetworkBehaviour
{
    [SerializeField]
    PlayerStatus _playerStatus;
    [SerializeField]
    private Animator animator;
    void Start()
    {
        if (HasInputAuthority)
        {

            _playerStatus.useEMP
                .Subscribe(emp => {
                    RPC_UseEMP(emp);
                }).AddTo(this);
        }
        else
        {
            _playerStatus.currentHp
                .Subscribe(hp => {
                    RPC_UpdateCurrentHp(hp);
                }).AddTo(this);
        }
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UseEMP(bool _isUse, RpcInfo info = default)
    {
        if (!info.IsInvokeLocal)
        {
            Debug.Log("Invoke:Local");
            if (_isUse)
                EMPEffectEmitter.Instance.EmitEffect();
            else
                EMPEffectEmitter.Instance.FinishEffect();
        }
        else
        {
            if (_isUse)
                EMPEffectEmitter.Instance.DisplayUseText();
            else
                EMPEffectEmitter.Instance.HideUseText();
            Debug.Log("Invoke:notLocal");
        }
    }
    public void SetBool(int id,bool value)
    {
        RPC_SetBool(id, value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetBool(int id, bool value)
    {
        animator.SetBool(id, value);
    }
    public void SetFloat(int id, float value)
    {
        RPC_SetFloat(id, value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetFloat(int id, float value)
    {
        animator.SetFloat(id, value);
    }

    public void SetLayerWeight(int id,float value)
    {
        RPC_SetLayerWeight(id, value);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetLayerWeight(int id, float value)
    {
        animator.SetLayerWeight(id, value);
    }

    public void ResetTrigger(int id)
    {
        RPC_ResetTrigger(id);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ResetTrigger(int id)
    {
        animator.ResetTrigger(id);
    }

    public void SetTrigger(int id)
    {
        RPC_SetTrigger(id);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetTrigger(int id)
    {
        animator.SetTrigger(id);
    }
    public void Play(int id)
    {
        RPC_Play(id);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Play(int id)
    {
        animator.Play(id);
    }
    public void CrossFade(int id, float value)
    {
        RPC_CrossFade(id,value);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_CrossFade(int id, float value)
    {
        animator.CrossFade(id,value);
    }
}
