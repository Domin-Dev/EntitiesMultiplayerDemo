using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using Unity.Mathematics;
using System.Net.Sockets;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;





public class ConnectionManager : MonoBehaviour
{
    private enum ConnectionMode
    {
        Host,
        Client    
    }
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [SerializeField] private TMP_InputField IPInputField;
    [SerializeField] private TMP_InputField PortInputField;


    private void OnEnable()
    {
        hostButton.onClick.AddListener(() => Run(ConnectionMode.Host));
        joinButton.onClick.AddListener(() => Run(ConnectionMode.Client));
    }

    private void OnDisable()
    {
        hostButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();        
    }

    private string ReadIP()
    {
        if(IPAddress.TryParse(IPInputField.text, out var address) && address.AddressFamily == AddressFamily.InterNetwork)
            return address.ToString();
        return "127.0.0.1";
    }
    private ushort ReadPort()
    {
        if(ushort.TryParse(PortInputField.text, out ushort port))
            return port;
        return 7979;
    }


    private void DestroyLocalWorld()
    {
        for (int i = World.All.Count - 1; i >= 0; i--)
        {
            World world = World.All[i];
            if(world.Flags == WorldFlags.GameClient || world.Flags == WorldFlags.GameServer)
            {
                world.Dispose();
            }
        }
    }
    private void Run(ConnectionMode mode)
    {
        DestroyLocalWorld();
        SceneManager.LoadScene(1);

        switch(mode)
        {
            case ConnectionMode.Host:
                StartServer();
                StartClient();
                break;
            case ConnectionMode.Client:
                StartClient();
                break;
        }
    }

    private void StartServer()
    {
        ushort port = ReadPort();

        var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld!");
        var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
        {
            using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }
    }
    private void StartClient()
    {
        ushort port = ReadPort();
        string address = ReadIP();

        var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld!");     
        var connectionEndpoint = NetworkEndpoint.Parse(address,port);
        {
            using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager,connectionEndpoint);
        }
        World.DefaultGameObjectInjectionWorld = clientWorld;
    }

}
