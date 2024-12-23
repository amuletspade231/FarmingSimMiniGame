// Client Manager
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance { get; private set; }

    private TextMeshProUGUI joinCodeText;
    private string joinCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void JoinGame(string lobbyCode)
    {
        try
        {
            // Set the join code
            joinCode = lobbyCode;

            // Initialize Unity Services
            await UnityServices.InitializeAsync();

            // Sign in anonymously if not already signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Join the Relay allocation using the join code
            JoinAllocation allocation = null;
            try
            {
                // Try to join the allocation using the join code
                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to join game with join code '{joinCode}': {ex.Message}");
                return; // Optionally, show a UI message to indicate the failure
            }

            // Set the relay server data for the transport
            var relayServerData = new RelayServerData(allocation, "dtls");
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayServerData);

            // Start the client to connect to the host
            NetworkManager.Singleton.StartClient();

            // Load the lobby scene or transition to the gameplay scene once connected
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error joining game: {e.Message}");
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")  // Check if the scene loaded is the lobby
        {
            // Try to find the UI Text object dynamically
            joinCodeText = GameObject.FindGameObjectWithTag("LobbyCode")?.GetComponent<TextMeshProUGUI>();

            // If the text object is found, update it with the join code
            if (joinCodeText != null)
            {
                joinCodeText.text = $"Join Code: {joinCode}";
            }
            else
            {
                Debug.LogError("Join Code Text object not found in the Lobby Scene!");
            }
        }
    }
}

