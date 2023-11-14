using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;
using Demo.Scripts.Runtime;
using InGame.Player;
using Kinemation.FPSFramework.Runtime.FPSAnimator;

public class PlayerRPCManager : NetworkBehaviour
{
    [SerializeField]
    PlayerStatus _playerStatus;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    NetworkFPSController _networkFPSController;
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
                EMPEffectEmitter.Instance.DisplayUseText(true);
            else
                EMPEffectEmitter.Instance.DisplayUseText(false);
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
    public void SetActionState(FPSActionState state)
    {
        RPC_SetActionState(state);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetActionState(FPSActionState state)
    {
        _networkFPSController.actionState = state;
    }
    public void PlayMotionAsset(IKAnimation animation)
    {
        RPC_PlayMotionAsset(animation.ToString());
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayMotionAsset(string animationName)
    {
        _networkFPSController.PlayMotionAsset(animationName);
    }

    public void SetLeanDirection(int value)
    {
        RPC_SetLeanDirection(value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetLeanDirection(int value)
    {
        _networkFPSController.SetLeanDirection(value);
    }

    public void SetMoveInput(Vector2 value)
    {
        RPC_SetMoveInput(value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetMoveInput(Vector2 value)
    {
        _networkFPSController.SetMoveInput(value);
    }
    public void SetAimInput(Vector2 value)
    {
        RPC_SetAimInput(value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetAimInput(Vector2 value)
    {
        _networkFPSController.SetAimInput(value);
    }
    public void SetAddDeltaInput(Vector2 value)
    {
        RPC_SetAddDeltaInput(value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetAddDeltaInput(Vector2 value)
    {
        _networkFPSController.SetAddDeltaInput(value);
    }
}
