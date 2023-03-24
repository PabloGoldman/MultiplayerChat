using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TcpConnection
{
    private readonly TcpClient connection;
    private NetworkStream stream;
    private IReceiveData receiver;
    private Queue<byte[]> dataReceivedQueue = new Queue<byte[]>();

    object handler = new object();

    public TcpConnection(string ipAddress, int port, IReceiveData receiver = null)
    {
        connection = new TcpClient(ipAddress, port);
        stream = connection.GetStream();
        this.receiver = receiver;
        BeginRead();
    }

    public void Close()
    {
        stream.Close();
        connection.Close();
    }

    public void FlushReceiveData()
    {
        lock (handler)
        {
            while (dataReceivedQueue.Count > 0)
            {
                byte[] dataReceived = dataReceivedQueue.Dequeue();
                if (receiver != null)
                    receiver.OnReceiveData(dataReceived, null);
            }
        }
    }

    private void BeginRead()
    {
        byte[] buffer = new byte[1024];
        stream.BeginRead(buffer, 0, buffer.Length, OnRead, buffer);
    }

    private void OnRead(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            byte[] buffer = (byte[])ar.AsyncState;
            byte[] dataReceived = new byte[bytesRead];
            Buffer.BlockCopy(buffer, 0, dataReceived, 0, bytesRead);

            lock (handler)
            {
                dataReceivedQueue.Enqueue(dataReceived);
            }

            BeginRead();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[TcpConnection] " + e.Message);
        }
    }

    public void Send(byte[] data)
    {
        stream.Write(data, 0, data.Length);
    }
}