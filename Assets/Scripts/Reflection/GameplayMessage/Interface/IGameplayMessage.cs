using System;
using System.Collections.Generic;

public class IntMessage : IMessage<int>
{
    int data;
    int clientID;

    public IntMessage(int data)
    {
        this.data = data;
    }

    public IntMessage(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public int Deserialize(byte[] message)
    {
        int outData;

        outData = BitConverter.ToInt32(message, 8);

        return outData;
    }

    public void SetClientId(int id)
    {
        clientID = id;
    }

    public int GetData()
    {
        return data;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(clientID));

        outData.AddRange(BitConverter.GetBytes(data));

        return outData.ToArray();
    }

    public MessageType GetMessageType()
    {
        return MessageType.intMessage;
    }
}