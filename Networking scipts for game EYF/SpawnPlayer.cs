using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class SpawnPlayer : NetworkBehaviour
{
    public GameObject player;
    public GameObject map;
    public GameObject mapPrefab;

    private PlayersInGameInfo PGI;

    // Start is called before the first frame update
    void Start()
    {
        PGI = GameObject.FindGameObjectWithTag("GM").GetComponent<PlayersInGameInfo>();

        if (SettingsScript.singlePlayerMode) 
        {
            GameObject newPlayer = Instantiate(player, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Quaternion.identity);
            Debug.Log("single");
            CamPosition.SetWatchedPlayer(newPlayer.transform);
            PGI.localPlayer = newPlayer;

        } 
        else
        {
            Destroy(map);
            SpawnLocalPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            
            if (NetworkManager.Singleton.IsHost)
            {
                SpawnMapServerRpc();
            }
        }
    }

    //SpawnPlayer
    [ServerRpc(RequireOwnership = false)]
    private void SpawnLocalPlayerServerRpc(ulong id)
    {
        Debug.Log("SpawnPlayer");
        Debug.Log(id);
        GameObject newPlayer = Instantiate(player, new Vector3(id * 2, 0, player.transform.position.z), Quaternion.identity);
        newPlayer.name = "Player" + id;

        NetworkObject NO = newPlayer.GetComponent<NetworkObject>();

        NO.SpawnWithOwnership(id);

        string name = "unknown";

        PlayerLobbyInfo? PI = LobbyManager.GetPlayerInfo(id);
        if (PI.HasValue)
        {
            name = PI.Value.PlName;
        }

        CalculatePlayersClientRpc(id, NO.NetworkObjectId, name);
    }

    
    [ClientRpc]
    private void CalculatePlayersClientRpc(ulong id, ulong newPlayerNetworkObjectId, string name)
    {
        Debug.Log("CalculatePlayersClientRpc");

        PGI.AddNewPlayer(id, name, newPlayerNetworkObjectId);
    }

    
    //SpawnMap
    [ServerRpc(RequireOwnership = false)]
    private void SpawnMapServerRpc()
    {
        GameObject newObj = Instantiate(mapPrefab, mapPrefab.transform.position, Quaternion.identity);

        newObj.transform.Find("Ground").GetComponent<NetworkObject>().Spawn();

        Debug.Log("mapSpawned");
    }
}
