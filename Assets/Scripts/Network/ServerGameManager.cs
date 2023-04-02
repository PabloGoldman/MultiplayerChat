using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerGameManager : MonoBehaviourSingleton<ServerGameManager>
{
    public GameObject cubePrefab;
    private Dictionary<int, GameObject> cubes = new Dictionary<int, GameObject>();

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();


    //private bool IsFreshMessage(NetMessage netMessage)
    //{
    //    if (!clients.ContainsKey(netMessage.clientId))
    //        return false;

    //    float elapsedTime = Time.realtimeSinceStartup - clients[netMessage.clientId].timeStamp;
    //    if (elapsedTime > NetworkManager.Instance.TimeOut)
    //        return false;

    //    return true;
    //}

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
}