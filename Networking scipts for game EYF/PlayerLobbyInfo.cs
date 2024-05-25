using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerLobbyInfo
{
    public string PlName { get; private set; }
    public bool LobbyReady { get; set; }

    public PlayerLobbyInfo(string playerName, bool isReady)
    {
        PlName = playerName;
        LobbyReady = isReady;
    }
}
