using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestClient : MonoBehaviour
{
    [Header("Components")]
    public Transform TargetTransform;
    
    [Header("Options")]
    public float PollFrequency = 1 / 60f;
    
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
        if (Time.time - _timeOfLastCheck < PollFrequency)
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

            const char endOfMessageMarker = ';';
            
            var messages = str.Split(endOfMessageMarker);
            
            for (var i = 0; i < messages.Length; i++)
            {
                ProcessMessage(messages[i]);
            }
        }
    }

    private void ProcessMessage(string str)
    {
        // Data should be sent in order of:
        // Roll, Pitch, Yaw, Button1, Button2
        var values = str.Split(',');
        
        if (values.Length <= 1) return;

        if (float.TryParse(values[0], out _))
        {
            var roll = float.Parse(values[0]);
            var pitch = float.Parse(values[1]);
            var yaw = float.Parse(values[2]);
            //TargetTransform.rotation = Quaternion.Euler(roll, yaw, pitch);
            TargetTransform.rotation = Euler(roll, yaw, pitch);
        }
        
        var isButton1Pressed = values[3] == "1";
        if (isButton1Pressed)
        {
            Debug.Log("Button 1 pressed");
        }
        
        var isButton2Pressed = values[4] == "1";
        if (isButton2Pressed)
        {
            Debug.Log("Button 2 pressed");
        }
    }

    private bool ConnectToHost()
    {
        try
        {
            _tcpClient.Connect(Host, Port);
            _networkStream = _tcpClient.GetStream();
            _streamWriter = new StreamWriter(_networkStream);
            
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
    
    public static Quaternion Euler(float yaw, float pitch, float roll)
    {
        yaw*=Mathf.Deg2Rad;
        pitch*=Mathf.Deg2Rad;
        roll*=Mathf.Deg2Rad;

        double yawOver2 = yaw * 0.5f;
        float cosYawOver2 = (float)System.Math.Cos(yawOver2);
        float sinYawOver2 = (float)System.Math.Sin(yawOver2);
        double pitchOver2 = pitch * 0.5f;
        float cosPitchOver2 = (float)System.Math.Cos(pitchOver2);
        float sinPitchOver2 = (float)System.Math.Sin(pitchOver2);
        double rollOver2 = roll * 0.5f;
        float cosRollOver2 = (float)System.Math.Cos(rollOver2);
        float sinRollOver2 = (float)System.Math.Sin(rollOver2);            
        Quaternion result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }
}
