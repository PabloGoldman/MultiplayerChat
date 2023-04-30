using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetThereIsNoPlace : IMessage<int>
{
    int data;

    public NetThereIsNoPlace(int data)
    {
        this.data = data;
    }

    public NetThereIsNoPlace(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public int GetData()
    {
        return data;
    }

    public int Deserialize(byte[] message)
    {
        int outData;

        outData = BitConverter.ToInt32(message, 4);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.ThereIsNoPlace;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data));

        return outData.ToArray();
    }
}
