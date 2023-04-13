using System;
using System.Collections.Generic;

[Serializable]
public class NetMessage
{
    char[] data;
    int clientId;

    public NetMessage(char[] data)
    {
        this.data = data;
    }

    public NetMessage(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    public char[] GetData()
    {
        return data;
    }

    public char[] Deserialize(byte[] message)
    {
        char[] outData = new char[(message.Length - 2 * sizeof(int)) / sizeof(char)];


        for (int i = 0 ; i < outData.Length; i++)
        {
            outData[i] = BitConverter.ToChar(message, (2 * sizeof(int) + i * sizeof(char))); 
            UnityEngine.Debug.Log(outData[i]);
        }


        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientId));

        for (int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
        }

        return outData.ToArray();
    }
}