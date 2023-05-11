using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRepeatMessage 
{
    int clientId;

    public NetRepeatMessage()
    {
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    public MessageType GetMessageType()
    {
        return MessageType.RepeatMessage; 
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientId));

        return outData.ToArray();
    }
}
