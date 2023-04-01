using UnityEngine;

public class Cube : MonoBehaviour
{
    public int clientId;
    public float speed = 5f;

    void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
            if (Input.GetKey(KeyCode.W))
            {
                MoveCube(speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                MoveCube(-speed);
            }
        }

        // Send the position of the cube to the server
        SendPosition();
    }

    public void SetClientId(int id)
    {
        clientId = id;
    }

    void MoveCube(float speed)
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void SendPosition()
    {
        NetVector3 netVector3 = new NetVector3(transform.position);
        netVector3.SetClientId(clientId);
        NetworkManager.Instance.SendToServer(netVector3.Serialize());
    }
}