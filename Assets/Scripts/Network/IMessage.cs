
public enum MessageType
{
    ThereIsNoPlace = -7,
    Disconnection = -6,
    NewCustomerNotice = -5,
    Position = -4,
    CheckActivity = -3,
    SetClientID = -2,
    HandShake = -1,
    Console = 0,
    intMessage = 1,
    floatMessage = 2,
    boolMessage  = 3,
    charMessage  = 4,
    vector3Message = 5,
};

public interface IMessage<T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
    public T GetData();
}


