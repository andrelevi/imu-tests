using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestClient : MonoBehaviour
{
    [Header("Network Settings")]
    public string LocalHost = "127.0.0.1";
    public string RemoteHost = "192.168.x.x";
    public bool UseRemoteHost;
    public int Port = 5005;

    [Header("Run-time Data")]
    private TcpClient TcpClient;
    private NetworkStream NetworkStream;
    private StreamWriter StreamWriter;
    private float _timeOfLastCheck;
    
    private string Host => UseRemoteHost ? RemoteHost : LocalHost;

    private void Start()
    {
        TcpClient = new TcpClient();

        if (ConnectToHost())
        {
            Debug.Log($"Connected to: {Host}:{Port}");
        }
    }

    private void Update()
    {
        if (Time.time - _timeOfLastCheck < 0.1f)
        {
            return;
        }

        _timeOfLastCheck = Time.time;
        
        if (!TcpClient.Connected)
        {
            ConnectToHost();
        }
        else
        {
            var stream = TcpClient.GetStream();
            var bytesToRead = new byte[TcpClient.ReceiveBufferSize];
            var bytesRead = stream.Read(bytesToRead, 0, this.TcpClient.ReceiveBufferSize);
            Debug.Log("Received: " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
        }
    }

    private bool ConnectToHost()
    {
        try
        {
            TcpClient.Connect(Host, Port);
            NetworkStream = TcpClient.GetStream();
            StreamWriter = new StreamWriter(NetworkStream);
            
            var sendBytes = Encoding.UTF8.GetBytes("Hello, I am client.");
            TcpClient.GetStream().Write(sendBytes, 0, sendBytes.Length);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        if (TcpClient != null && TcpClient.Connected)
        {
            TcpClient.Close();
        }
    }
}
