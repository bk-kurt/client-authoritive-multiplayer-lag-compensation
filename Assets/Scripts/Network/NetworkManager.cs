using System;
using Riptide.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public enum ServerToClientId : ushort
{
    Movement = 1,
}

public enum ClientToServerId : ushort
{
    Input = 1,
}

public class NetworkManager : MonoSingleton<NetworkManager>
{
    public MyClient Client { get; private set; }
    public MyServer Server { get; private set; }

    [Header("Server Settings")] 
    public float serverTickRate = 60f;
    [SerializeField] private ushort maxClientCount = 2;

    [Header("Client Side Prediction")] [SerializeField]
    public float inputMessageDelay = 100f;

    [SerializeField] public float packetLossChance = 10f;

    public const int SimulationStateCacheSize = 1024;

    private void Awake()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, 
            Debug.LogError, false);
    }

    private void Start()
    {
        Server = new MyServer();
        Server.Start(8989, maxClientCount);

        Client = new MyClient();
        Client.Connect("127.0.0.1:8989");
    }

    private void FixedUpdate()
    {
        if (Server.IsRunning)
        {
            Server.Tick();
        }

        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
        Client.Disconnect();
    }
}