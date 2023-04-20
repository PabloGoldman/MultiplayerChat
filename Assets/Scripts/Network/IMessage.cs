using System;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    SetClientID = -2,
    HandShake = -1,
    Console = 0,
    Position = 1,
    NewCustomerNotice = 2,
    Disconnection = 3
};

public interface IMessage <T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}


