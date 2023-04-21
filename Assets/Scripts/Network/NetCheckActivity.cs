using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCheckActivity : MonoBehaviour
{
    int clientId;

    public NetCheckActivity()
    {
    }

    //    public NetDisconnection(byte[] data)
    //    {
    //        this.data = Deserialize(data);
    //    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    //    public char[] GetData()
    //    {
    //        return data;
    //    }

    //    public char[] Deserialize(byte[] message)
    //    {
    //    
    //
    //        return outData;
    //    }

    public MessageType GetMessageType()
    {
        return MessageType.CheckActivity;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientId));

        return outData.ToArray();
    }
}
