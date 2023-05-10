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
        }


        return outData;
    }

    public static void Deserialize(byte[] message, out char[] outData, out int sum)
    {
        outData = new char[(message.Length - 2 * sizeof(int)) / sizeof(char)];
        int dataSize = outData.Length * sizeof(char);

        for (int i = 0; i < outData.Length; i++)
        {
            outData[i] = BitConverter.ToChar(message, 2 * sizeof(int) + i * sizeof(char));
        }

        sum = BitConverter.ToInt32(message, sizeof(int) + dataSize );
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

        int sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
            sum += (int)data[i];
        }

        outData.AddRange(BitConverter.GetBytes(sum));

        return outData.ToArray();
    }
}