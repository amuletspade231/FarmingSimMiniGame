// PlayerData.cs
using System;
using Unity.Netcode;

[Serializable]
public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;
    // Player's ready up variable
    public NetworkVariable<string> Ready = new NetworkVariable<string>("false");
    // Player's score as a network variable to sync across the network to all players
    public NetworkVariable<int> Score = new NetworkVariable<int>(0);  
}

