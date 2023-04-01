using System;

[Serializable]
public class NetMessage
{
    public int clientId;
    public int messageId;
    public object message;

    public NetMessage(int clientId, int messageId, object message)
    {
        this.clientId = clientId;
        this.messageId = messageId;
        this.message = message;
    }
}