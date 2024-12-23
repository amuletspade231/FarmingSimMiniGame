// Host Manager
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HostManager : MonoBehaviour
{

    public static HostManager Instance {  get; private set; }

    // TextBox to display lobby join code
    public TextMeshProUGUI joinCodeText;

    [SerializeField] private string joinCode; // Variable to store the join code
    public string playerNumber = "Unknown";

    // UI elements for the ready button and ready status text
    public Button readyButton;
    public TextMeshProUGUI readyStatusText;

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
 
    public async void StartHostRun()
    {
        GameManager.instance.isMultiplayer = true;
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Multiplayer Services Initialized");

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation;
            try
            {
                // Attempt Relay allocation for hosting
                allocation = await RelayService.Instance.CreateAllocationAsync(GameManager.instance.maxPlayers);
            }
            catch (System.Exception ex)
            {
                // If allocation fails, log the error and provide feedback
                Debug.LogError($"Failed to create Relay allocation: {ex.Message}");
                return;
            }

            // Try to get the join code
            try
            {
                // Get the join code after successful allocation
                joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Join Code: {joinCode}");
            }
            catch (System.Exception ex)
            {
                // If getting the join code fails, log the error and provide feedback
                Debug.LogError($"Failed to get join code: {ex.Message}");
                return; 
            }
            Debug.Log("Relay Initialized");

            var relayServerData = new RelayServerData(allocation, "dtls");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayServerData);

            // Start hosting the game
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

            // Start hosting with Relay data
            NetworkManager.Singleton.StartHost();

            // Load the lobby scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");

            // Register the disconnect callback
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;

            SceneManager.sceneLoaded += OnLobbySceneLoaded;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting host: {e}");
        }
    }


    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {

        ulong clientId = request.ClientNetworkId; // Extract client ID from the request

        if (GameManager.instance.Players.Count < GameManager.instance.maxPlayers && !GameManager.instance.Players.ContainsKey(clientId))
        {
            response.Approved = true;  // Directly set approval to true
            playerNumber = "Player" + (GameManager.instance.Players.Count + 1); // Example default name
            GameManager.instance.AddPlayer(clientId, playerNumber);
        }
        else
        {
            response.Approved = false; // Deny the connection if the conditions aren't met
        }

        response.CreatePlayerObject = response.Approved; // If approved, create a player object
        response.Pending = false; 
    }

    private void HandleDisconnect(ulong clientId)
    {
        Debug.Log("Client Disconnecting...");
        if (NetworkManager.Singleton.IsHost)
        {
            // Inform all clients and end the game
            Debug.Log("Host has disconnected. Ending game...");
            // Switch the game state to GAME_END
            GameManager.instance.ChangeState(GameManager.GameState.GAME_END);
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            // Handle the disconnection of other clients (you already have this in place)
            NetworkManager.Singleton.DisconnectClient(clientId);
            GameManager.instance.RemovePlayer(clientId, playerNumber);
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (playerObject != null)
            {
                playerObject.Despawn();
            }
        }

    }

    private void OnLobbySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if we're in the lobby scene
        if (scene.name == "LobbyScene")
        {
            // Search for the TextMeshPro component in the scene by tag
            joinCodeText = GameObject.FindGameObjectWithTag("LobbyCode")?.GetComponent<TextMeshProUGUI>();

            // If found, update the text with the join code
            if (joinCodeText != null)
            {
                joinCodeText = joinCodeText.GetComponent<TextMeshProUGUI>();
                if (joinCodeText != null && !string.IsNullOrEmpty(joinCode))
                {
                    joinCodeText.text = $"Join Code: {joinCode}";
                }
            }
            else
            {
                Debug.Log("joinCodeText is null and can not update");
            }

            // Link the ready button UI
            readyButton = GameObject.FindGameObjectWithTag("ReadyButton")?.GetComponent<Button>();
            if (readyButton != null)
            {
                Debug.Log($"Button found {readyButton}");
            }
            // Link the ready status text UI
            readyStatusText = GameObject.FindGameObjectWithTag("ReadyText")?.GetComponent<TextMeshProUGUI>();
            if (readyStatusText != null)
            {
                Debug.Log($"Text found {readyStatusText}");
            }
            // Ensure that the ready button is not null before adding the listener
            if (readyButton != null)
            {
                readyButton.onClick.AddListener(PlayerReady);
            }


            // Spawn the player object for the host
            if (NetworkManager.Singleton.IsHost)
            {
                GameObject playerObject = Instantiate(NetworkManager.Singleton.NetworkConfig.PlayerPrefab, Vector3.zero, Quaternion.identity);
                NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    // Spawn the player object and set ownership to the host
                    networkObject.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);
                    networkObject.ChangeOwnership(0);
                }
                else
                {
                    Debug.LogError("PlayerPrefab does not have a NetworkObject attached.");
                }
            }
        }

        // Unsubscribe from the scene loaded event after it’s done
        SceneManager.sceneLoaded -= OnLobbySceneLoaded;
    }

    // Unsubscribe from the events to prevent memory leaks or unwanted callback triggers
    private void OnApplicationQuit()
    {
        // Unsubscribe from the connection approval callback
        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;

        // Unsubscribe from the client disconnect callback
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;

    }
    private void PlayerReady()
    {
        GameManager.instance.ChangeState(GameManager.GameState.GAME_START);
        Debug.Log("Player Readying up");
        /*if (GameManager.instance.Players.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            // Get the current player's data
            var playerData = GameManager.instance.Players[NetworkManager.Singleton.LocalClientId];

            if (playerData != null)
            {
                Debug.Log($"Player Data found, player ready status set to: {playerData.Ready.Value}");
                // Check if the player is already ready or not
                if (playerData.Ready.Value == "true")
                {
                    // Player was already ready, now mark them as not ready
                    playerData.Ready.Value = "false"; // Set to "false"
                    readyStatusText.text = "Ready up?"; 
                }
                else if (playerData.Ready.Value == "false")
                {
                    // Player was not ready, now mark them as ready
                    playerData.Ready.Value = "true"; // Set to "true"
                    readyStatusText.text = "Ready!";
                }
                else
                {
                    Debug.LogError("Player data is null.");
                }

                // Now check if all players are ready
                bool allReady = true;

                // Iterate through all players in the GameManager's dictionary
                foreach (var pd in GameManager.instance.GetAllPlayerData())
                {
                    if (pd.Ready.Value == "false")
                    {
                        allReady = false;
                        break;
                    }
                }

                // If all players are ready, start the game
                if (allReady)
                {
                    Debug.Log("All players are ready! Starting the game.");
                    // Change the game state to GAME_START
                    GameManager.instance.ChangeState(GameManager.GameState.GAME_START);
                }
            }
        }*/
    }
}

