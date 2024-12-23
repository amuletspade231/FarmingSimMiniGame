// Multiplayer Menu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.PackageManager;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_InputField joinCodeInput;

    private void Awake()
    {
        //init buttons and null checks for the buttons
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);
        /*if (serverButton != null)
            serverButton.onClick.AddListener(StartServer);
        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient); */
        if (joinLobbyButton != null)
            joinLobbyButton.onClick.AddListener(JoinLobby);

    }
    
    private void StartHost()
    {
        GameManager.instance.isMultiplayer = true;
        Debug.Log("Host button pressed");
        HostManager.Instance.StartHostRun();  
    }

    private void JoinLobby()
    {
        // Get the join code entered by the user
        string joinCode = joinCodeInput.text;

        // Validate the join code 
        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("Join code cannot be empty.");
            return; 
        }

        ClientManager.Instance.JoinGame(joinCode); 
    }

    private void OnDestroy()
    {
        // Cleanin up the listeners
        if (hostButton != null) hostButton.onClick.RemoveListener(StartHost);
        /* if (serverButton != null) serverButton.onClick.RemoveListener(StartServer);
        if (clientButton != null) clientButton.onClick.RemoveListener(StartClient); */
        if (joinLobbyButton != null) joinLobbyButton.onClick.RemoveListener(JoinLobby);
    }
    /*   private void StartServer()
       {
           Debug.Log("Server button pressed");
           NetworkManager.Singleton.StartServer();
           NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

       }
        private void StartClient()
        {
            Debug.Log("Client button pressed");
            ClientManager.Instance.StartClientRun();
        }
    */

}


