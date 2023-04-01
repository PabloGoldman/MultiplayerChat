using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class MessageChecker : MonoBehaviourSingleton<MessageChecker>
{
    public MessageType CheckMessageType(byte[] message)
    {
        int messageType = 0;

        messageType = BitConverter.ToInt32(message, 0);

        return (MessageType)messageType;
    }

    public int CheckClientId(byte[] message)
    {
        int clientId = 0;

        clientId = BitConverter.ToInt32(message, 4);

        return clientId;
    }
}
