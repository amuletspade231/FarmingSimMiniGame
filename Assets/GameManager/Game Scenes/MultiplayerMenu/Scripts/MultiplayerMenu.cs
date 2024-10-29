using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "GameplayExample";
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        //init buttons and null checks for the buttons
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);
        if (serverButton != null)
            serverButton.onClick.AddListener(StartServer);
        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient);
    }
    
    private void StartHost()
    {
        Debug.Log("Host button pressed");
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
    
    private void StartServer()
    {
        Debug.Log("Server button pressed");
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

    }

    private void StartClient(){
        Debug.Log("Client button pressed");
        NetworkManager.Singleton.StartClient();
        

    }
    
    private void OnDestroy()
    {
        // Cleanin up the listeners
        if (hostButton != null) hostButton.onClick.RemoveListener(StartHost);
        if (serverButton != null) serverButton.onClick.RemoveListener(StartServer);
        if (clientButton != null) clientButton.onClick.RemoveListener(StartClient);
    }
}
