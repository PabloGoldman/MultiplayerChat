using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetNewCustomerNotice : IMessage<(long, int)>
{
    int clientId;
    (long, int) data;

    public NetNewCustomerNotice((long, int) data)
    {
        this.data = data;
    }
    public NetNewCustomerNotice(byte[] data)
    {
        this.data = Deserialize(data);
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
        return MessageType.NewCustomerNotice;
    }

    public (long, int) GetData()
    {
        return data;
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
