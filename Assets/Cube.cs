using System;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour , IMessage<Vector3>
{
    Vector3 data;

    public Cube(Vector3 data)
    {
        this.data = data;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {

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
