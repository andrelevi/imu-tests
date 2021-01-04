using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class IMUServerConnection : MonoBehaviour
    {
        [Header("Components")]
        public Controller Controller;
    
        [Header("Network Settings")]
        public string LocalHost = "127.0.0.1";
        public string RemoteHost = "192.168.x.x";
        public bool UseRemoteHost;
        public int Port = 5005;
        [Tooltip("How often the IMU is polled for data.")] public float PollFrequency = 1 / 60f;

        [Header("Run-time Data")]
        private TcpClient _tcpClient;
    
        private string Host => UseRemoteHost ? RemoteHost : LocalHost;

        private void Start()
        {
            Controller.MessageFrequency = PollFrequency;
            
            _tcpClient = new TcpClient();

            if (ConnectToHost())
            {
                Debug.Log($"Connected to: {Host}:{Port}");

                StartCoroutine(_Poll());
            }
        }

        private bool ConnectToHost()
        {
            try
            {
                _tcpClient.Connect(Host, Port);
                
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Socket error: " + e);
            
                return false;
            }
        }

        private IEnumerator _Poll()
        {
            while (_tcpClient.Connected)
            {
                var stream = _tcpClient.GetStream();
                
                if (stream.DataAvailable)
                {
                    var bytesToRead = new byte[_tcpClient.ReceiveBufferSize];
                
                    var bytesRead = stream.Read(bytesToRead, 0, _tcpClient.ReceiveBufferSize);
                    var str = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                    const char endOfMessageMarker = ';';
                
                    var messages = str.Split(endOfMessageMarker);
                
                    for (var i = 0; i < messages.Length; i++)
                    {
                        Controller.ProcessMessage(messages[i]);
                    }
                }
                
                yield return new WaitForSeconds(PollFrequency);
            }
        }

        private void DisconnectFromServer()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Close();
            }
        }

        private void OnApplicationQuit()
        {
            DisconnectFromServer();
        }
        
        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                DisconnectFromServer();
            }
        }
    }
}
