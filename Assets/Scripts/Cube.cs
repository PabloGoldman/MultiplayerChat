using System.Net;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int clientId;
    public float speed = 5f;

    //  [Net] public int vida = 10;


    private void Start()
    {
        clientId = transform.GetComponent<Player>().clientId;
    }
    void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
           if (NetworkManager.Instance.actualClientId == clientId)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    MoveCube(Vector3.right, speed);
                }

                if (Input.GetKey(KeyCode.A))
                {
                    MoveCube(Vector3.left , speed);
                }

                if (Input.GetKey(KeyCode.W))
                {
                    MoveCube(Vector3.up, speed);
                }

                if (Input.GetKey(KeyCode.S))
                {
                    MoveCube(Vector3.down, speed);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    MoveCube(Vector3.forward, speed);
                }

                if (Input.GetKey(KeyCode.E))
                {
                    MoveCube(Vector3.back, speed);
                }

                // Manda la posicion al server
                SendPosition();
            }
        }
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    void MoveCube(Vector3 direction, float speed)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void SendPosition()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        {

            NetVector3 netVector3 = new NetVector3(transform.position);
            netVector3.SetClientId(clientId);

            NetworkManager.Instance.SendToServer(netVector3.Serialize());
            NetVector3.SetLastMessage(NetVector3.GetLastMessage() + 1);
        }
    }
}