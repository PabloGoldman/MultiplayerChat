using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    bool alreadyAddedClient;
    public int actualClientId = 0;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    // Prefab del cubo que se va a generar al conectarse un cliente
    public GameObject cubePrefab;
    private Dictionary<int, GameObject> cubes = new Dictionary<int, GameObject>();

    public int clientId; // This id should be generated during first handshake
    static int lastMessageRead = 0;

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake handShakeMesage = new NetHandShake((ip.Address, port));
        handShakeMesage.SetClientId(clientId);
        SendToServer(handShakeMesage.Serialize());

        if (!alreadyAddedClient)
        {
            actualClientId = clientId;
            alreadyAddedClient = true;
        }

        AddClient(new IPEndPoint(ip, port));
    }

    public void AddClient(IPEndPoint ip)
    {
        if (!ipToId.ContainsKey(ip))
        {
            Debug.Log("Adding client: " + ip.Address);

            clients.Add(clientId, new Client(ip, clientId, Time.realtimeSinceStartup));

            // Se genera un cubo para el cliente que se acaba de conectar
            GenerateCubeForClient(clientId);

            clientId++;
        }
    }

    private void GenerateCubeForClient(int clientId)
    {
        GameObject cube = Instantiate(cubePrefab);
        cube.name = "Cube_" + clientId;
        cube.GetComponent<Cube>().clientId = clientId;
        cubes.Add(clientId, cube);
    }

    private void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);

            // Se destruye el cubo del cliente que se desconectó
            Destroy(GameObject.Find("Cube_" + ipToId[ip]));
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);

        int clientId = MessageChecker.Instance.CheckClientId(data);

        switch (MessageChecker.Instance.CheckMessageType(data))
        {
            case MessageType.HandShake:

                NetHandShake handShake = new NetHandShake(data);
                handShake.SetClientId(clientId);

                //if (ip != newIp)
                if (!clients.ContainsKey(clientId))
                {
                    // AddClient(new IPEndPoint(handShake.getData().Item1, handShake.getData().Item2));
                    AddClient(ip);

                    BroadcastCubePosition(clientId, handShake.Serialize());
                }
                else
                {
                    Debug.Log("Es el mismo cliente");
                }

                break;
            case MessageType.Console:

                break;
            case MessageType.Position:

                lastMessageRead++; //Tengo qe guardar un diccionario para saber qe cliente es el ultimo mensaje
                int currentMessage = NetVector3.GetLastMessage();

                if (lastMessageRead < currentMessage)
                {
                    // Debug.Log("Se perdio el mensaje = " + lastMessageRead);
                    lastMessageRead = currentMessage;
                }
                else
                {
                    UpdateCubePosition(cubes[clientId].GetComponent<Cube>().clientId, data);
                }

                break;

            default:

                Debug.Log("No llego ningun dato con \"MessaggeType\"");

                break;
        }

        if (!isServer)
        {
            Debug.Log("Esta recibiendo datos un cliente");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            NetVector3 net = new NetVector3(Vector3.zero);
            net.SetClientId(1);
            Broadcast(net.Serialize(), clients[1].ipEndPoint);
        }

    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void Broadcast(byte[] data, IPEndPoint ip)
    {
        connection.Send(data, ip);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    private void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }

    private void UpdateCubePosition(int clientId, byte[] data)
    {
        if (!cubes.ContainsKey(clientId))
        {
            Debug.LogWarning("Cube for client ID " + clientId + " not found in the dictionary.");
            return;
        }

        // Get the cube GameObject for the client
        GameObject cube = cubes[clientId];

        // Deserialize the payload into a NetVector3
        NetVector3 netPosition = new NetVector3(data);


        // Set the cube's position to the position received from the client
        cube.transform.position = netPosition.GetData();

        // Broadcast the cube's position to all other clients
        BroadcastCubePosition(clientId, data);
    }

    private void BroadcastCubePosition(int senderClientId, byte[] data)
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
                    Broadcast(data, clients[receiverClientId].ipEndPoint);
                }
            }
        }
    }




}