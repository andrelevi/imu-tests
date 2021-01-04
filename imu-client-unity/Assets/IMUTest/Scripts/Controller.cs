using System;
using DG.Tweening;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class Controller : MonoBehaviour
    {
        public Camera Camera;
        public Menu Menu;
        
        [Header("Components")]
        public Transform Button1;
        public Transform Button2;
        
        [Header("Controller Visualizer")]
        public float ButtonYInactive;
        public float ButtonYPressed;

        [Header("Spawn")]
        public Vector3 SpawnOffsetFromCamera;
        
        [Header("Run-time Data")]
        [NonSerialized] public float MessageFrequency;
        private float _timeOfLastMessage;
        private Quaternion _rotationOffset;
        private bool _hasSetRotationOffset;
        private Quaternion _rotationTarget;
        private bool _isButton1Pressed;
        private bool _isButton2Pressed;
        private Tween _button1Tween;
        private Tween _button2Tween;
        
        private void SetRotationOffset()
        {
            _rotationOffset = transform.rotation;
        }
        
        public void ProcessMessage(string str)
        {
            // Data should be sent in order of:
            // Roll, Pitch, Yaw, Button1, Button2
            var values = str.Split(',');
        
            if (values.Length < 5) return;

            var roll = float.Parse(values[0]);
            var pitch = float.Parse(values[1]);
            var yaw = float.Parse(values[2]);
            //_rotationTarget = MathUtils.Euler(roll, pitch, yaw);
            _rotationTarget = Quaternion.Euler(pitch, yaw, roll);

            if (!_hasSetRotationOffset)
            {
                SetRotationOffset();
        
                _hasSetRotationOffset = true;
            }
            else
            {
                _rotationTarget = Quaternion.Inverse(_rotationOffset) * _rotationTarget;
            }
            
            var isButton1Pressed = values[3] == "1";
            if (GetButtonDown(1, isButton1Pressed))
            {
                Menu.ResetItems();
                _button1Tween?.Kill();
                _button1Tween = Button1.DOLocalMoveY(ButtonYPressed, 0.1f);
            }
            else if (GetButtonUp(1, isButton1Pressed))
            {
                _button1Tween?.Kill();
                _button1Tween = Button1.DOLocalMoveY(ButtonYInactive, 0.1f);
            }
            
            if (isButton1Pressed)
            {
                var pos = transform.TransformPoint(new Vector3(0, 0, 5));
                
                if (Menu.CanSpawnItemAtPosition(pos))
                {
                    Menu.SpawnItemAtPosition(pos);
                }
            }
            
            _isButton1Pressed = isButton1Pressed;
        
            var isButton2Pressed = values[4] == "1";
            if (GetButtonDown(2, isButton2Pressed))
            {
                SetRotationOffset();
                SpawnRelativeToCamera();
                
                _button2Tween?.Kill();
                _button2Tween = Button2.DOLocalMoveY(ButtonYPressed, 0.1f);
            }
            else if (GetButtonUp(2, isButton2Pressed))
            {
                _button2Tween?.Kill();
                _button2Tween = Button2.DOLocalMoveY(ButtonYInactive, 0.1f);
            }
            _isButton2Pressed = isButton2Pressed;
            
            _timeOfLastMessage = Time.time;
        }

        private void Update()
        {
            var interpolationDuration = MessageFrequency;
            var extrapolationDuration = MessageFrequency;
            
            var t = (Time.time - _timeOfLastMessage) / interpolationDuration;

            if (t > 1)
            {
                t = Mathf.Clamp(t, 0, 1 + (extrapolationDuration / MessageFrequency));
            }
            
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, _rotationTarget, t);
        }

        private void SpawnRelativeToCamera()
        {
            // Only use the yaw rotation of the camera to determine spawn position.
            var cameraEulerAngles = Camera.transform.eulerAngles;
            cameraEulerAngles.x = 0;
            cameraEulerAngles.z = 0;
            
            var cameraRotation = Quaternion.Euler(cameraEulerAngles);

            transform.position = Camera.transform.position + cameraRotation * SpawnOffsetFromCamera;
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
    }
}
