using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestClient : MonoBehaviour
{
    public Transform TargetTransform;
    
    [Header("Network Settings")]
    public string LocalHost = "127.0.0.1";
    public string RemoteHost = "192.168.x.x";
    public bool UseRemoteHost;
    public int Port = 5005;

    [Header("Run-time Data")]
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamWriter _streamWriter;
    private float _timeOfLastCheck;
    
    private string Host => UseRemoteHost ? RemoteHost : LocalHost;

    private void Start()
    {
        _tcpClient = new TcpClient();

        if (ConnectToHost())
        {
            Debug.Log($"Connected to: {Host}:{Port}");
        }
    }

    private void Update()
    {
        if (Time.time - _timeOfLastCheck < 1f)
        {
            return;
        }

        _timeOfLastCheck = Time.time;
        
        if (!_tcpClient.Connected)
        {
            ConnectToHost();
        }
        else
        {
            var stream = _tcpClient.GetStream();
            var bytesToRead = new byte[_tcpClient.ReceiveBufferSize];
            var bytesRead = stream.Read(bytesToRead, 0, _tcpClient.ReceiveBufferSize);
            var str = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            var values = str.Split(',');
            
            Debug.Log(str);
            //Debug.Log(values.Length);

            if (float.TryParse(values[0], out _))
            {
                TargetTransform.eulerAngles = new Vector3(
                    float.Parse(values[0]),
                    float.Parse(values[1]),
                    float.Parse(values[2])
                );
            }
        }
    }

    private bool ConnectToHost()
    {
        try
        {
            _tcpClient.Connect(Host, Port);
            _networkStream = _tcpClient.GetStream();
            _streamWriter = new StreamWriter(_networkStream);
            
            //var sendBytes = Encoding.UTF8.GetBytes("Hello, I am client.");
            //_tcpClient.GetStream().Write(sendBytes, 0, sendBytes.Length);
            
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
        if (_tcpClient != null && _tcpClient.Connected)
        {
            _tcpClient.Close();
        }
    }
}
