using UnityEngine;

public class Cube : MonoBehaviour
{
    public int clientId;
    public float speed = 5f;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveCube(speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveCube(-speed);
        }

        // Send the position of the cube to the server
        SendPosition();
    }

    void MoveCube(float speed)
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void SendPosition()
    {
        NetVector3 netVector3 = new NetVector3(transform.position);
        NetworkManager.Instance.SendToServer(netVector3.Serialize());
    }
}