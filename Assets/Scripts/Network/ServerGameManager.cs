using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerGameManager : MonoBehaviourSingleton<ServerGameManager>
{
    public GameObject cubePrefab;
    private Dictionary<int, GameObject> cubes = new Dictionary<int, GameObject>();

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    private int clientId = 0;

    private void Start()
    {
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnReceiveEvent -= OnReceiveData;
    }

    private void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        // Deserialize NetMessage and check if message is fresh
        MessageChecker messageChecker = new MessageChecker();

        //if (!IsFreshMessage(netMessage))
        //    return;

        int clientId = 0;

        clientId = messageChecker.CheckClientId(data);

        switch (messageChecker.CheckMessageType(data))
        {
            case MessageType.HandShake:
                AddClient(ip, clientId);
                break;
            case MessageType.Console:
                break;
            case MessageType.Position:
                UpdateCubePosition(clientId, data);
                break;
            default:
                break;
        }
    }

    private bool IsFreshMessage(NetMessage netMessage)
    {
        if (!clients.ContainsKey(netMessage.clientId))
            return false;

        float elapsedTime = Time.realtimeSinceStartup - clients[netMessage.clientId].timeStamp;
        if (elapsedTime > NetworkManager.Instance.TimeOut)
            return false;

        return true;
    }

    private void AddClient(IPEndPoint ip, int clientId)
    {
        if (!ipToId.ContainsKey(ip))
        {
            ipToId[ip] = clientId;
            clients.Add(clientId, new Client(ip, clientId, Time.realtimeSinceStartup));

            // Spawn a new cube for the client
            GameObject cube = Instantiate(cubePrefab);
            cubes.Add(clientId, cube);
        }
    }

    private void UpdateCubePosition(int clientId, byte[] payload)
    {
        // Get the cube GameObject for the client
        GameObject cube = cubes[clientId];

        // Deserialize the payload into a NetVector3
        NetVector3 netPosition = new NetVector3(payload);

        // Set the cube's position to the position received from the client
        cube.transform.position = netPosition.GetData();

        // Broadcast the cube's position to all other clients
        BroadcastCubePosition(clientId, payload);
    }

    private void BroadcastCubePosition(int senderClientId, byte[] payload)
    {
        // Iterate through all clients and send the position update to each client
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                int receiverClientId = iterator.Current.Key;

                // No te automandes tu propio movimiento
                if (receiverClientId != senderClientId)
                {
                    NetworkManager.Instance.Broadcast(payload, clients[receiverClientId].ipEndPoint);
                }
            }
        }
    }
}