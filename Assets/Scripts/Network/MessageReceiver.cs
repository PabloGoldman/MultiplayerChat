using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageReceiver : MonoBehaviour
{
    public MessageReceiver()
    { }

    public void NewMessage(byte[] message)
    {
        int messageType = 0;

        messageType = BitConverter.ToInt32(message, 0);

        switch ((MessageType)messageType)
        {
            case MessageType.Console:


                break;
            case MessageType.Position:

                //NetVector3 netVector3 = new NetVector3(message);

                //UnityEngine.Vector3 positionReceived = netVector3.getData();

                //De alguna manera le tengo que mandar la posicion al cubito

                break;

            case MessageType.HandShake:

                //NetHandShake netHandShake = new NetHandShake(message);

                //NetworkManager.Instance.AddClient(new IPEndPoint(netHandShake.getData().Item1, netHandShake.getData().Item2));

                break;

            default:
                break;
        }
    }
}
