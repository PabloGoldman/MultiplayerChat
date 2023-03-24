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
                NetworkManager.Instance.Broadcast(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}
