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
        char[] outData = new char[message.Length / sizeof(char)/* - sizeof(int) * 2*/]; //Ojo aca puede estar mal el tamaño!!

        for (int i = 0; i < outData.Length; i++)
        {
            outData[i] = BitConverter.ToChar(message, (i * sizeof(char))/* + sizeof(int) * 2*/);  //El size of int lo hago para que no tome los primeros 8 bytes que dan el tipo de mensaje y id
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