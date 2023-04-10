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

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    // Prefab del cubo que se va a generar al conectarse un cliente
    public GameObject cubePrefab;
    private Dictionary<int, GameObject> cubes = new Dictionary<int, GameObject>();

    public int serverClientId = 0; // Se genera en elñ primer handshake
    public int actualClientId = 0;
    static Dictionary<int, int> lastMessageRead = new Dictionary<int, int>();

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
        handShakeMesage.SetClientId(0);
        SendToServer(handShakeMesage.Serialize());
    }

    public void AddClient(IPEndPoint ip, int newClientID)
    {
        if (!ipToId.ContainsKey(ip))
        {
            Debug.Log("Adding client: " + ip.Address);

            clients.Add(newClientID, new Client(ip, newClientID, Time.realtimeSinceStartup));
            lastMessageRead.Add(newClientID, 0);

            // Se genera un cubo para el cliente que se acaba de conectar
            GenerateCubeForClient(newClientID);

            if (isServer)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    NetNewCustomerNotice netNewCoustomer = new NetNewCustomerNotice((clients[i].ipEndPoint.Address.Address, clients[i].ipEndPoint.Port));
                    netNewCoustomer.SetClientId(i);
                    Broadcast(netNewCoustomer.Serialize()); 
                }
            }
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

        int messageId = MessageChecker.Instance.CheckClientId(data);

        switch (MessageChecker.Instance.CheckMessageType(data))
        {
            case MessageType.HandShake:

                NetHandShake handShake = new NetHandShake(data);
                handShake.SetClientId(serverClientId);

                if (!clients.ContainsKey(serverClientId))
                {
                    NetSetClientID netSetClientID = new NetSetClientID(serverClientId);
                    Broadcast(netSetClientID.Serialize(), ip);

                    AddClient(ip, serverClientId);

                    NetNewCustomerNotice netNewCoustomer = new NetNewCustomerNotice(handShake.getData());
                    netNewCoustomer.SetClientId(serverClientId);
                    BroadcastCubePosition(serverClientId, netNewCoustomer.Serialize());
                    serverClientId++;
                }
                else
                {
                    Debug.Log("Es el mismo cliente");
                }

                break;
            case MessageType.Console:

                break;

            case MessageType.NewCoustomerNotice:

                if (!clients.ContainsKey(messageId))
                {
                    NetNewCustomerNotice NewCoustomer = new NetNewCustomerNotice(data);
                    AddClient(ip, messageId);
                }

                break;

            case MessageType.SetClientID:

                NetSetClientID netGetClientID = new NetSetClientID(data);

                actualClientId = netGetClientID.GetData();

                AddClient(ip, actualClientId);

                break;

            case MessageType.Position:


                lastMessageRead[messageId]++;
                int currentMessage = NetVector3.GetLastMessage();

                if (lastMessageRead[messageId] < currentMessage)
                {
                    Debug.Log("Se perdio el mensaje = " + lastMessageRead);
                    lastMessageRead[messageId] = currentMessage;
                }
                else
                {
                    UpdateCubePosition(cubes[messageId].GetComponent<Cube>().clientId, data);
                }

                break;

            default:

                Debug.Log("No llego ningun dato con \"MessaggeType\"");

                break;
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

        // Toma el cubo del cliente
        GameObject cube = cubes[clientId];

        // Deserealiza la data en un NetVector3
        NetVector3 netPosition = new NetVector3(data);

        // Setea la posicion del cubo a la posicion que recibio del cliente
        cube.transform.position = netPosition.GetData();

        // Broadcastea la posicion del cubo al resto de los clientes

        BroadcastCubePosition(clientId, data);
    }

    private void BroadcastCubePosition(int senderClientId, byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                int receiverClientId = iterator.Current.Key;

                // Evita que te mandes tuu propia posicion
                if (receiverClientId != senderClientId)
                {
                    //Chequea ambos IpEndPoint, y si el enviador es el mismo que el receptor, continua el loop sin hacer el Broadcast
                    if (clients[receiverClientId].ipEndPoint.Equals(clients[senderClientId].ipEndPoint)) continue;
                    Broadcast(data, clients[receiverClientId].ipEndPoint);

                }
            }
        }
    }
}