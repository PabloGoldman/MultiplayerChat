using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    public int serverClientId = 0; // Se genera en el primer handshake
    public int actualClientId = 0;
    static Dictionary<int, int> lastMessageRead = new Dictionary<int, int>();

    int timeUntilDisconnection = 30;
    private Dictionary<int, int> lastMessageReceivedFromClients = new Dictionary<int, int>();
    int lastMessageReceivedFromServer = 0;

    private Dictionary<long, float> lastLatencyReceivedFromClients = new Dictionary<long, float>();
    float lastLatencyReceivedFromServer = 0;
    float currentLatency;
   
    
    int maximumNumberOfUsers = 2;
    float timeOutServer = 15;
    float timer = 0;

    bool nextServerIsActive = false;
    bool firtStartClient = true;
    Process nextServerApplication;

    string serverBuildPath;

    void Awake()
    {
        string baseFolderPath = Path.GetFullPath(Application.dataPath + "/../..");
        serverBuildPath = baseFolderPath + "/Server/MultiplayerChat.exe";

        //string serverBuildDirectory = Path.GetDirectoryName(serverBuildPath);
        //Directory.CreateDirectory(serverBuildDirectory);

        // serverBuildPath = "C:/Users/Admin/Desktop/Proyectos Unity/MultiplayerChatProjects/Builds/Server/MultiplayerChat.exe";

        UnityEngine.Debug.Log(serverBuildPath);
#if UNITY_SERVER
        port = 51000;

        try
        {
            int portNumber;
            int.TryParse(System.Environment.GetCommandLineArgs()[1], out portNumber);

            if (portNumber != port)
            {
                port = portNumber;
            }
        }
        catch (Exception)
        {

            Console.WriteLine("Falla");

        }

        Console.WriteLine("--------------------------------------------");
        Console.WriteLine("\n" + "\n" + "\n" + port + "\n" + "\n" + "\n");
        Console.WriteLine("--------------------------------------------");

        StartServer(port);
#endif
    }

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);

        InvokeRepeating(nameof(AddTime), 1.0f, 1.0f);
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

        UnityEngine.Debug.Log(firtStartClient);
        if (firtStartClient)
        {
            InvokeRepeating(nameof(AddTime), 1.0f, 1.0f);
            firtStartClient = false;
        }
    }

    public void AddClient(IPEndPoint ip, int newClientID)
    {
        if (!clients.ContainsKey(newClientID))
        {
            UnityEngine.Debug.Log("Adding client: " + ip.Address);

            clients.Add(newClientID, new Client(ip, newClientID, Time.realtimeSinceStartup));
            lastMessageRead.Add(newClientID, 0);
            lastMessageReceivedFromClients.Add(newClientID, 0);
            lastLatencyReceivedFromClients.Add(newClientID, 0);

            // Se genera un cubo para el cliente que se acaba de conectar
            GenerateCubeForClient(newClientID);

            if (isServer)
            {

                using (var iterator = clients.GetEnumerator())
                {
                    while (iterator.MoveNext())
                    {
                        int receiverClientId = iterator.Current.Key;

                        NetNewCustomerNotice netNewCoustomer = new NetNewCustomerNotice((clients[receiverClientId].ipEndPoint.Address.Address, clients[receiverClientId].ipEndPoint.Port));
                        netNewCoustomer.SetClientId(receiverClientId);
                        Broadcast(netNewCoustomer.Serialize());

                    }
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

    private void RemoveClient(int idToRemove)
    {
        if (clients.ContainsKey(idToRemove))
        {
            Destroy(cubes[idToRemove]);

            clients.Remove(idToRemove);
            cubes.Remove(idToRemove);
            lastMessageRead.Remove(idToRemove);
            lastMessageReceivedFromClients.Remove(idToRemove);
            lastLatencyReceivedFromClients.Remove(idToRemove);
        }
    }

    bool CheckServerIsFull()
    {
        return clients.Count >= maximumNumberOfUsers ? true : false;
    }

    IEnumerator CreateNewServer(IPEndPoint ip)
    {
        int numberPort = port;
        numberPort++;

        CreateServerProcess(numberPort);

        nextServerIsActive = true;

        //Aca habria un loading screen o algo asi
        yield return new WaitForSeconds(8.0f);

        NetThereIsNoPlace thereIsNoPlace = new NetThereIsNoPlace(numberPort);
        Broadcast(thereIsNoPlace.Serialize(), ip);
    }

    void CreateServerProcess(int numberPort)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();

        startInfo.FileName = serverBuildPath;
        startInfo.Arguments = numberPort.ToString();

        nextServerApplication = Process.Start(startInfo);
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        int messageId = MessageChecker.Instance.CheckClientId(data);

        switch (MessageChecker.Instance.CheckMessageType(data))
        {
            case MessageType.HandShake:
                if (CheckServerIsFull())
                {
                    UnityEngine.Debug.Log(nextServerIsActive);

                    if (nextServerIsActive) //si existe
                    {
                        SendServerIsFullMessage(ip);
                    }
                    else
                    {
                        StartCoroutine(CreateNewServer(ip));
                    }
                }
                else
                {
                    ConnectToServer(data, ip);
                }
                break;

            case MessageType.Console:

                UpdateChatText(data);

                break;

            case MessageType.NewCustomerNotice:

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

                if (CheckForLastMessage(messageId, currentMessage))
                {
                    lastMessageRead[messageId] = currentMessage;
                }
                else
                {
                    UpdateCubePosition(cubes[messageId].GetComponent<Cube>().clientId, data);
                }
                break;

            case MessageType.Disconnection:

                if (isServer)
                {
                    Broadcast(data);
                    RemoveClient(messageId);
                }
                else
                {
                    RemoveClient(messageId);
                }
                break;

            case MessageType.CheckActivity:

                if (isServer)
                {
                    lastMessageReceivedFromClients[messageId] = 0;
                }
                else
                {
                    lastMessageReceivedFromServer = 0;
                }


                NetCheckActivity netCheckActivity = new NetCheckActivity(data);

                currentLatency = DateTime.Now.Ticks - netCheckActivity.GetData().Item1;

                if (isServer)
                {
                    lastLatencyReceivedFromClients[messageId] = currentLatency;
                }
                else
                {
                    lastLatencyReceivedFromServer = currentLatency;
                }

                break;

            case MessageType.ThereIsNoPlace:

                MoveToNextPortServer(data);
                break;

            default:
                UnityEngine.Debug.Log("No llego ningun dato con \"MessaggeType\"");
                break;
        }
    }

    private bool CheckForLastMessage(int messageId, int currentMessage)
    {
        return lastMessageRead[messageId] < currentMessage;
    }

    private void UpdateChatText(byte[] data)
    {
        NetMessage netMessage = new NetMessage(data);

        if (isServer)
        {
            Broadcast(data);
        }

        string text = "";
        char[] aux = netMessage.GetData();

        for (int i = 0; i < netMessage.GetData().Length; i++)
        {
            text += aux[i];
        }

        ChatScreen.Instance.messages.text += text + System.Environment.NewLine;
    }

    private void SendServerIsFullMessage(IPEndPoint ip)
    {
        int numberPort = port;
        numberPort++;
        NetThereIsNoPlace thereIsNoPlace = new NetThereIsNoPlace(numberPort);
        Broadcast(thereIsNoPlace.Serialize(), ip);
    }

    private void MoveToNextPortServer(byte[] data)
    {
        connection.Close();
        NetThereIsNoPlace netThereIsNoPlace = new NetThereIsNoPlace(data);
        StartClient(ipAddress, netThereIsNoPlace.GetData());
    }

    private void ConnectToServer(byte[] data, IPEndPoint ip)
    {
        NetHandShake handShake = new NetHandShake(data);
        handShake.SetClientId(serverClientId);

        //Chequea que no sea un cliente que ya exista
        if (!clients.ContainsKey(serverClientId))
        {
            //Le asigna un ID al cliente y despues lo broadcastea
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
            UnityEngine.Debug.Log("Es el mismo cliente");
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

        CheckNextServerActivity();
    }

    private void FixedUpdate()
    {
        if (connection == null)
            return;


        SendCheckMessageActivity();
    }

    void CheckNextServerActivity() => nextServerIsActive = (nextServerApplication != null && !nextServerApplication.HasExited);
    //{
    //    nextServerIsActive = (nextServerApplication != null && !nextServerApplication.HasExited);
    //}

    void AddTime()
    {
        if (isServer)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    int receiverClientId = iterator.Current.Key;

                    lastMessageReceivedFromClients[receiverClientId]++;
                }
            }
        }
        else
        {
            lastMessageReceivedFromServer++;
        }

        if (clients.Count == 0 && port != 51000)
        {
            timer++;
            if (timer >= timeOutServer)
            {
                Application.Quit();
            }
        }
        else
        {
            timer = 0;
        }
    }

    void SendCheckMessageActivity()
    {
        if (isServer)
        {
            NetCheckActivity netCheckActivity = new NetCheckActivity((DateTime.Now.Ticks, currentLatency));
            netCheckActivity.SetClientId(-1);

            Broadcast(netCheckActivity.Serialize());
        }
        else
        {
            NetCheckActivity netCheckActivity = new NetCheckActivity((DateTime.Now.Ticks, currentLatency));
            netCheckActivity.SetClientId(actualClientId);

            SendToServer(netCheckActivity.Serialize());
        }

        if (isServer)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    int receiverClientId = iterator.Current.Key;

                    if (lastMessageReceivedFromClients[receiverClientId] >= timeUntilDisconnection)
                    {
                        RemoveClient(receiverClientId);

                        NetDisconnection netDisconnection = new NetDisconnection();
                        netDisconnection.SetClientId(receiverClientId);

                        Broadcast(netDisconnection.Serialize());
                    }
                }
            }
        }
        else
        {
            if (lastMessageReceivedFromServer >= timeUntilDisconnection)
            {
                NetDisconnection netDisconnection = new NetDisconnection();
                netDisconnection.SetClientId(actualClientId);

                SendToServer(netDisconnection.Serialize());
            }
        }
    }

    private void UpdateCubePosition(int clientId, byte[] data)
    {
        if (!cubes.ContainsKey(clientId))
        {
            UnityEngine.Debug.LogWarning("Cube for client ID " + clientId + " not found in the dictionary.");
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

    void OnApplicationQuit()
    {
        if (!isServer)
        {
            NetDisconnection netDisconnection = new NetDisconnection();
            netDisconnection.SetClientId(actualClientId);

            UnityEngine.Debug.Log("Lo expulso por falta de actividad");

            SendToServer(netDisconnection.Serialize());
        }
    }
}