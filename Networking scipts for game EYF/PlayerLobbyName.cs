using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;

public class PlayerLobbyName : NetworkBehaviour
{
    public TextMeshProUGUI displayNameOBJ;
    public Toggle displayToggleOBJ;

    private NetworkVariableString displayName = new NetworkVariableString();
    private NetworkVariableBool displayReady = new NetworkVariableBool();

    public override void NetworkStart()
    {
        if (!IsServer) { return; }

        PlayerLobbyInfo? playerData = LobbyManager.GetPlayerInfo(OwnerClientId);

        if (playerData.HasValue) 
        {
            displayName.Value = playerData.Value.PlName;
            displayReady.Value = playerData.Value.LobbyReady;
        }        
    }

    private void Update()
    {
        if (!IsServer) { return; }

        PlayerLobbyInfo? playerData = LobbyManager.GetPlayerInfo(OwnerClientId);

        if (playerData.HasValue)
        {
            displayReady.Value = playerData.Value.LobbyReady;
        }
    }

    private void OnEnable()
    {
        displayName.OnValueChanged += HandleNameChanged;
        displayReady.OnValueChanged += HandleReadyChanged;
    }

    private void OnDisable()
    {
        displayName.OnValueChanged -= HandleNameChanged;
        displayReady.OnValueChanged -= HandleReadyChanged;
    }

    private void HandleNameChanged(string oldName, string newName)
    {
        displayNameOBJ.text = newName;
    }

    private void HandleReadyChanged(bool oldBool, bool newBool)
    {
        displayToggleOBJ.isOn = newBool;
    }
}
