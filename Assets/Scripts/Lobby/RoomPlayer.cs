using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static RoomPlayer Local;
    private PlayerRef _ref;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log(Launcher._runner.GameMode);
        }

        Players.Add(this);
        Debug.Log(Object.InputAuthority);

        DontDestroyOnLoad(gameObject);
    }

    public static void RemovePlayer(NetworkRunner runner, PlayerRef player)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == player);
        if (roomPlayer != null)
        {
            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
    }
}
