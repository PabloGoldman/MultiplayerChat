using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCheckActivity : IMessage<(long, float)>
{
    int clientId;
    (long, float) data;

    public NetCheckActivity((long, float) data)
    {
        this.data = data;
    }

    public NetCheckActivity(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }
    public (long, float) GetData()
    {
        return data;
    }

    public (long, float) Deserialize(byte[] message)
    {
        (long, float) outdata;

        outdata.Item1 = BitConverter.ToInt64(message, 8);
        outdata.Item2 = BitConverter.ToInt32(message, 16);

        return outdata;
    }

    public MessageType GetMessageType()
    {
        return MessageType.CheckActivity;
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
