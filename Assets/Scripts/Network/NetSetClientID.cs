using System;
using System.Collections.Generic;

public class NetSetClientID : IMessage<int>
{

    int data;

    public NetSetClientID(int data)
    {
        this.data = data;
    }

    public NetSetClientID(byte[] data)
    {
        this.data = Deserialize(data);
    }

    public int GetData()
    {
        return data;
    }

    public int Deserialize(byte[] message)
    {
        int outData;

        outData = BitConverter.ToInt32(message, 4);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.SetClientID;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data));
       
        return outData.ToArray();
    }
}


