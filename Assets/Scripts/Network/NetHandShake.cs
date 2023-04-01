using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetHandShake : IMessage<(long, int)>
{
    (long, int) data;
    public (long, int) Deserialize(byte[] message)
    {
        (long, int) outdata;

        outdata.Item1 = BitConverter.ToInt64(message, 4);
        outdata.Item2 = BitConverter.ToInt32(message, 12);

        return outdata;
    }

    public MessageType GetMessageType()
    {
        return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        return outData.ToArray();
    }
}
