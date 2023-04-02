using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    Vector3 data;
    static int lastMessage = 0;
    int clientId;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public NetVector3(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    public Vector3 GetData()
    {
        return data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 12);
        outData.y = BitConverter.ToSingle(message, 16);
        outData.z = BitConverter.ToSingle(message, 20);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public static int GetLastMessage()
    {
        return lastMessage;
    }
    public static void SetLastMessage(int value)
    {
        lastMessage = value;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientId));
        outData.AddRange(BitConverter.GetBytes(lastMessage));


        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }
}
