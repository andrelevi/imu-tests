﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class IMUServerConnection : MonoBehaviour
    {
        [Header("Components")]
        public Transform TargetTransform;
        public Transform Button1;
        public Transform Button2;
    
        [Header("Options")]
        public float PollFrequency = 1 / 60f;
    
        [Header("Controller Visualizer")]
        public float ButtonYInactive;
        public float ButtonYPressed;
    
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
        private bool _isButton1Pressed;
        private bool _isButton2Pressed;
        private Tween _button1Tween;
        private Tween _button2Tween;
        private Quaternion _rotationOffset;
        private bool _hasSetRotationOffset;
    
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

        private void ResetRotationOffset()
        {
            _rotationOffset = TargetTransform.rotation;
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
                //TargetTransform.rotation = Euler(roll, yaw, pitch);
                TargetTransform.rotation = Euler(roll, pitch, yaw);
            }

            if (!_hasSetRotationOffset)
            {
                ResetRotationOffset();
        
                _hasSetRotationOffset = true;
            }
            else
            {
                TargetTransform.rotation = Quaternion.Inverse(_rotationOffset) * TargetTransform.rotation;
            }
        
            var isButton1Pressed = values[3] == "1";
            if (GetButtonDown(1, isButton1Pressed))
            {
                //SetBasis();
                _button1Tween?.Kill();
                _button1Tween = Button1.DOLocalMoveY(ButtonYPressed, 0.1f);
            }
            else if (GetButtonUp(1, isButton1Pressed))
            {
                _button1Tween?.Kill();
                _button1Tween = Button1.DOLocalMoveY(ButtonYInactive, 0.1f);
            }
            _isButton1Pressed = isButton1Pressed;
        
            var isButton2Pressed = values[4] == "1";
            if (GetButtonDown(2, isButton2Pressed))
            {
                //SetBasis();
                _button2Tween?.Kill();
                _button2Tween = Button2.DOLocalMoveY(ButtonYPressed, 0.1f);
            }
            else if (GetButtonUp(2, isButton2Pressed))
            {
                _button2Tween?.Kill();
                _button2Tween = Button2.DOLocalMoveY(ButtonYInactive, 0.1f);
            }
            _isButton2Pressed = isButton2Pressed;
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

        private bool GetButtonDown(int index, bool isPressed)
        {
            switch (index)
            {
                case 1:
                    return !_isButton1Pressed && isPressed;
                
                case 2:
                    return !_isButton2Pressed && isPressed;
            }
        
            return false;
        }
    
        private bool GetButton(int index)
        {
            switch (index)
            {
                case 1:
                    return _isButton1Pressed;
                
                case 2:
                    return _isButton2Pressed;
            }
        
            return false;
        }
    
        private bool GetButtonUp(int index, bool isPressed)
        {
            switch (index)
            {
                case 1:
                    return _isButton1Pressed && !isPressed;
                
                case 2:
                    return _isButton2Pressed && !isPressed;
            }
        
            return false;
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
}