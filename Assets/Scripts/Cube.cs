using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Cube : MonoBehaviour, IMessage<Vector3>
{
    Vector3 data;
    byte[] byteData = new byte[3 * sizeof(float)];

    public float speed = 5f;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    public Cube(Vector3 data)
    {
        this.data = data;
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }

        transform.position = Deserialize(data);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveCube();
            SendData();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
        }
    }

    void MoveCube()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void SendData()
    {
        data = transform.position;

        byteData = Serialize(); //Le pasamos la data del vector, a bytes
        transform.position = Deserialize(byteData); //Pasamos la data en bytes, a tipo Vec3 y se la asignamos a la posicion

        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(byteData);
            Debug.Log("isServer");
        }
        else
        {
            NetworkManager.Instance.SendToServer(byteData);
            Debug.Log("isNotServer");
        }
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 4);
        outData.y = BitConverter.ToSingle(message, 8);
        outData.z = BitConverter.ToSingle(message, 12);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }
}
