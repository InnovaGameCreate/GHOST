using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UniRx;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    NetworkRunner _runner;
    PlayerRef _playerPref;
    PlayerStatus _playerStatus;
    [Networked] private TickTimer reSpawnTime { get; set; }
    [Networked] private TickTimer despawnTime { get; set; }
    NetworkObject networkPlayerObject;
    private SpawnPoint _spawnPoint;
    private bool isAlive = false;
    PlayerUiPresenter _playerUiPresenter;
    public void SetInitialData(NetworkRunner runner, PlayerRef playerPre)
    {
        _playerUiPresenter = FindObjectOfType<PlayerUiPresenter>();
        _spawnPoint = FindObjectOfType<SpawnPoint>();
        _runner = runner;
        _playerPref = playerPre;
        Spawn();
    }
    private void Spawn()
    {
        networkPlayerObject = _runner.Spawn(_playerPrefab, _spawnPoint.GetSpawnPoint(), Quaternion.identity, _playerPref);
        _playerStatus = networkPlayerObject.GetComponent<PlayerStatus>();
        _playerStatus.setLocalCharacter();
        isAlive = true;
        RPC_SetHpBar();
        _playerStatus.currentHp
            .Subscribe(hp =>
            {
                if(hp <= 0)
                {
                    Despawn();
                }
            }).AddTo(this);
    }
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            if (despawnTime.Expired(Runner) && isAlive)
            {
                Runner.Despawn(networkPlayerObject);
                isAlive = false;
                ReSpawn();
                despawnTime = TickTimer.None;
            }
            if (reSpawnTime.Expired(Runner) && !isAlive)
            {
                RPC_DeadEffect(false);
                Spawn();
                reSpawnTime = TickTimer.None;
            }
        }
    }
    private void Despawn()
    {
        RPC_DeadEffect(true);
        despawnTime = TickTimer.CreateFromSeconds(Runner, 0.5f);
    }
    private void ReSpawn()
    {
        reSpawnTime = TickTimer.CreateFromSeconds(Runner, 3);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetHpBar()
    {
        _playerUiPresenter.setHpBar();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeadEffect(bool isDead)
    {
        if (HasInputAuthority)
        {
            EMPEffectEmitter.Instance.DeadEffect(true, isDead);
        }
        else
        {
            EMPEffectEmitter.Instance.DeadEffect(false, isDead);
        }
    }
}
