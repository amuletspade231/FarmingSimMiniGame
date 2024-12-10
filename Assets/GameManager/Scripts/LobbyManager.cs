using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("Lobby Details")]
    public string lobbyName;
    public int maxPlayers;

    [Header("Testing Buttons")]
    public bool createLobby = false;
    public bool isLobbyPrivate;
    public bool listLobbies = false;
    public bool joinFirstLobby = false;
    public bool joinLobbyByCode = false;
    public string lobbyCode;
    public bool leaveLobby = false;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float updateLobbyTimer;

    

    private async void Start()
    {
        // Initialize Unity online services
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed with ID: " + AuthenticationService.Instance.PlayerId);
        };
        // Users are annonymous - create account
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    // Handle testing buttons - should be moved to UI buttons later
    public void Update()
    {
        if (createLobby)
            CreateLobby();
        if (listLobbies)
            ListLobbies();
        if (joinFirstLobby)
            JoinLobby();
        if (joinLobbyByCode)
            JoinLobbyByCode(lobbyCode);
        if (leaveLobby)
            LeaveLobby();

        HandleHeartbeat();
        HandleLobbyUpdateTimer();
    }

    // Send a ping to keep the lobby open and not close every 30 sec
    private async void HandleHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float timerMax = 20;
                heartbeatTimer = timerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyUpdateTimer()
    {
        if (joinedLobby != null)
        {
            updateLobbyTimer -= Time.deltaTime;
            if (updateLobbyTimer < 0f)
            {
                float timerMax = 1.1f;
                updateLobbyTimer = timerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    // Create a new lobby  
    private async void CreateLobby()
    {
        // Testing bool
        createLobby = false;
        // Create a lobby option to set if it is public or private
        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        lobbyOptions.IsPrivate = isLobbyPrivate;
        lobbyOptions.Player = NewPlayer();

        // See if lobby is created
        try {
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            hostLobby = lobby;

            Debug.Log("Lobby Created: " + lobby.Name + " - " + lobby.MaxPlayers + " max players - Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(hostLobby);

        } catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // List all current lobbies
    private async void ListLobbies()
    {
        // Testing bool
        listLobbies = false;

        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            // Print all lobbies
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " max players");
            }
        } catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // Create new player object
    private Player NewPlayer()
    {
        return new Player { 
            Data = new Dictionary<string, PlayerDataObject> {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Player")}
            }
        };
    }
    

    // Join a lobby
    // Join first available lobby
    private async void JoinLobby()
    {
        // Testing bool
        joinFirstLobby = false;

        try
        {
            JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions();
            joinOptions.Player = NewPlayer();

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, joinOptions);
            joinedLobby = lobby;

            Debug.Log("Attempting to join a lobby");
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // Join a lobby
    // Join lobby by code
    private async void JoinLobbyByCode(string code)
    {
        // Testing bool
        joinLobbyByCode = false;

        try
        {
            JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions();
            joinOptions.Player = NewPlayer();

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinOptions);
            joinedLobby = lobby;

            Debug.Log("Attempting to join a lobby with code: " + code);
            PrintPlayers(joinedLobby);
        } catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            Debug.Log("No lobby code");
        }
    }

    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    // Print players in lobby
    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in current lobby: ");
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value );
        }
    }

    // Leave a lobby
    private async void LeaveLobby()
    {
        // Testing bool
        leaveLobby = false;

        try { 
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Player Left.");
            PrintPlayers();
        } catch (LobbyServiceException ex) {
            Debug.Log(ex);
        }
    }

}

