using System.Net;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }

        messages.text += System.Text.ASCIIEncoding.UTF8.GetString(data) + System.Environment.NewLine;
    }

    private void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {

            if (NetworkManager.Instance.isServer)
            {
                NetMessage netMessage = new NetMessage(str.ToCharArray());

                NetworkManager.Instance.Broadcast(netMessage.Serialize());
                messages.text += str + System.Environment.NewLine;
            }
            else
            {
                NetMessage netMessage = new NetMessage(str.ToCharArray());
                netMessage.SetClientId(NetworkManager.Instance.actualClientId);

                NetworkManager.Instance.SendToServer(netMessage.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}
