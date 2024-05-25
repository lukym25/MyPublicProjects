using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine.UI;
using TMPro;
using MLAPI.SceneManagement;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public GameObject lobby;
    public GameObject chooseLobby;

    public TextMeshProUGUI playerNameInput;
    public TextMeshProUGUI playerIpAdressInput;
    public TextMeshProUGUI placeHolderName;
    public TextMeshProUGUI placeHolderIpAdress;
    public TextMeshProUGUI passwordInput;
    public TextMeshProUGUI portInput;

    public GameObject hostSettings;
    public GameObject startBtn;
    public GameObject readyBtm;
    public GameObject unreadyBtm;
    public Button startBtnComp;
        

    public int MaxPlayers = 8;

    public GameObject scrollClinetContent;
    public GameObject hostSlot;
    public TextMeshProUGUI count;

    public UNetTransport UnetT;
    /*
    public NetworkObject playerLobbyIconPrefab;

    public NetworkObject playerCharacterPrefab;

    private NetworkObject spawnObject;*/

    private static Dictionary<ulong, PlayerLobbyInfo> clientData;

    private void Update()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            count.text = "" + NetworkManager.Singleton.ConnectedClients.Count;

            Debug.Log(MaxPlayers);
        } 
        
        //we need to add one because the client is not counting himself
        else if(NetworkManager.Singleton.IsClient)
        {
            int plNum = NetworkManager.Singleton.ConnectedClients.Count + 1;
            count.text = "" + plNum;
        }
    }


    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnectCallback;

        UnetT = NetworkManager.Singleton.GetComponent<UNetTransport>();      
    }
    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnectCallback;
    }

    //is called when we pressed host button
    public void Host()
    {
        if (NetworkManager.Singleton.IsClient) 
        {
            NetworkManager.Singleton.StopClient();
        }

        if(playerNameInput.text.Length < 4) 
        {
            placeHolderName.text = "Name is necessery please enter it (with at least 3 char)";
            playerNameInput.text = "";
            return;
        }

        string portI = HandyScript.ClearLastChar(portInput.text);
        if (portI.Length == 4)
        {
            UnetT.ConnectPort = int.Parse(portI);
        }

        //add host to clientData Dictionary
        clientData = new Dictionary<ulong, PlayerLobbyInfo>();
        clientData[NetworkManager.Singleton.LocalClientId] = new PlayerLobbyInfo(playerNameInput.text, true);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCallback;

        SettingsScript.singlePlayerMode = false;

        NetworkManager.Singleton.StartHost();
    }

    //is called when we pressed client button
    public void Client()
    {
        if (playerNameInput.text.Length < 4) 
        {
            placeHolderName.text = "Name is necessery please enter it (with at least 3 char)";
            playerNameInput.text = "";
            return;
        }

        if(playerIpAdressInput.text.Length <= 1)
        {
            UnetT.ConnectAddress = "127.0.0.1";
        } 
        else
        {
            UnetT.ConnectAddress = HandyScript.ClearLastChar(playerIpAdressInput.text);
        }

        string portI = HandyScript.ClearLastChar(portInput.text);
        if (portI.Length == 4)
        {
            UnetT.ServerListenPort = int.Parse(portI);
        }

        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }

        //pack file with player info to Json
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            name = HandyScript.ClearLastChar(playerNameInput.text),
            password = HandyScript.ClearLastChar(passwordInput.text)
        }); 

        //pack transfer Json file into bytes whitch can be transported by network
        byte[] payloadInBytes = System.Text.Encoding.ASCII.GetBytes(payload);

        //transfer payloadInBytes by network to host => ApprovalCallback arg1 bytes
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadInBytes;

        NetworkManager.Singleton.StartClient();

        SettingsScript.singlePlayerMode = false;
    }

    //is called when we pressed leave button
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCallback;
            NetworkManager.Singleton.StopHost();

            hostSettings.SetActive(false);
            startBtn.SetActive(false);
        } 
        if (NetworkManager.Singleton.IsClient)
        {

            NetworkManager.Singleton.StopClient();
        }


        readyBtm.SetActive(true);
        unreadyBtm.SetActive(false);


        chooseLobby.SetActive(true);
        lobby.SetActive(false);

        SettingsScript.singlePlayerMode = true;
    }

    //is called when client wants to join the server
    public void ApprovalCallback(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callBack)
    {
        string payloadInJson = System.Text.Encoding.ASCII.GetString(connectionData);
        ConnectionPayload payload = JsonUtility.FromJson<ConnectionPayload>(payloadInJson);

        bool isAllowed = true;
        if(NetworkManager.ConnectedClients.Count >= MaxPlayers) { isAllowed = false; }

        string password = HandyScript.ClearLastChar(passwordInput.text);
        if (password.Length > 0) 
        {
            if (password != payload.password)  { isAllowed = false; }
        }

        clientData[clientId] = new PlayerLobbyInfo(payload.name, false);

        callBack(true, null, isAllowed, null, null);
    }

    //is called when server started
    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleConnectedCallback(NetworkManager.Singleton.LocalClientId);
            hostSettings.SetActive(true);
            startBtn.SetActive(true);
            readyBtm.SetActive(false);
        } 
    }

    //is called when any of players join
    private void HandleConnectedCallback(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            chooseLobby.SetActive(false);
            lobby.SetActive(true);
        }

        if (NetworkManager.Singleton.IsHost)
        {
            UpdateScrollFieldClientRpc();

            ChackIfAllReady();
        }
    }

    //is called when any of players leave
    private void HandleClientDisconnectCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            clientData.Remove(clientId);
        }
    }

    public static PlayerLobbyInfo? GetPlayerInfo(ulong clientId)
    {
        if (clientData.TryGetValue(clientId, out PlayerLobbyInfo playerData))
        {
            return playerData;
        }

        return null;
    }
    
    [ClientRpc]
    private void UpdateScrollFieldClientRpc()
    {
        GameObject[] playerIcons = GameObject.FindGameObjectsWithTag("PlIcon");

        playerIcons[0].transform.SetParent(hostSlot.transform);
        playerIcons[0].transform.GetChild(1).gameObject.SetActive(false);

        for (int i = 1; i < playerIcons.Length; i++)
        {
            playerIcons[i].transform.SetParent(scrollClinetContent.transform);
        }
    }

    //buttons pressed change lobby ready state
    public void ReadyPressed()
    {
        ChangeReadyServerRpc(true, NetworkManager.LocalClientId);
    }

    public void UnreadyPressed()
    {
        ChangeReadyServerRpc(false, NetworkManager.LocalClientId);
    }

    public void StartGamePressed()
    {
        StartGameClientRpc();
        NetworkSceneManager.SwitchScene("Game");
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        SceneLoader.indexLevel = 1;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeReadyServerRpc(bool isOn, ulong id)
    {
        clientData[id] = new PlayerLobbyInfo(clientData[id].PlName, isOn);

        ChackIfAllReady();
    }

    private void ChackIfAllReady()
    {
        bool everyoneIsReady = true;

        //#2
        if (NetworkManager.Singleton.ConnectedClients.Count < 1) { everyoneIsReady = false; }

        foreach(PlayerLobbyInfo pi in clientData.Values)
        {
            if(pi.LobbyReady == false)
            {
                everyoneIsReady = false;
            }
        }
        startBtnComp.interactable = everyoneIsReady;
    }

    //settings change 
    public void ChangeMaxPlNum(string newMaxNum)
    {
        MaxPlayers =  int.Parse(newMaxNum);
        /*Debug.Log(NetworkManager.Singleton.ConnectedClients.Count + "/ " + MaxPlayers);

        int plDisconneting = NetworkManager.Singleton.ConnectedClients.Count - MaxPlayers; 

        for(int i = 0; i < plDisconneting; i++)
        {
            ulong plDisId = (ulong)(NetworkManager.Singleton.ConnectedClients.Count - i);
            DisconectClientRpc(plDisId);
        }
       */
    }
    /*
    [ClientRpc]
    public void DisconectClientRpc(ulong disconectId)
    {
        Debug.Log("somedic");
        if (NetworkManager.LocalClientId == disconectId)
        {
            Debug.Log("dicon");
            Leave();
        }
    } */
}
