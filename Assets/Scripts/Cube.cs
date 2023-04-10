using System.Net;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int clientId;
    public float speed = 5f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
           if (NetworkManager.Instance.actualClientId == clientId)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    MoveCube(speed);
                }

                if (Input.GetKey(KeyCode.A))
                {
                    MoveCube(-speed);
                }

                // Send the position of the cube to the server
                SendPosition();
            }
        }
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    void MoveCube(float speed)
    {
        //rb.AddForce(Vector3.right * speed);
        transform.position += Vector3.right * speed * Time.deltaTime;
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