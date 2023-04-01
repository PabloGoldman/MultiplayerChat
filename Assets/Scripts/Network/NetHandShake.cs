using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetHandShake : IMessage<(long, int)>
{
    //long = id
    //int = puerto

    int clientId;

    (long, int) data;

    public NetHandShake((long, int) data)
    {
        this.data = data;
    }

    public (long, int) Deserialize(byte[] message)
    {
        (long, int) outdata;

        outdata.Item1 = BitConverter.ToInt64(message, 8);
        outdata.Item2 = BitConverter.ToInt32(message, 16);

        return outdata;
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    public MessageType GetMessageType()
    {
        return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientId));
        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        return outData.ToArray();
    }
}
