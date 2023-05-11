using System;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    CheckActivity = -3,
    SetClientID = -2,
    HandShake = -1,
    Console = 0,
    Position = 1,
    NewCustomerNotice = 2,
    Disconnection = 3,
    ThereIsNoPlace = 4,
    RepeatMessage = 5
};

public interface IMessage <T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}


