using DG.Tweening;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class Controller : MonoBehaviour
    {
        [Header("Components")]
        public Transform Button1;
        public Transform Button2;
        
        [Header("Controller Visualizer")]
        public float ButtonYInactive;
        public float ButtonYPressed;
        
        [Header("Run-time Data")]
        private bool _isButton1Pressed;
        private bool _isButton2Pressed;
        private Tween _button1Tween;
        private Tween _button2Tween;
        private Quaternion _rotationOffset;
        private bool _hasSetRotationOffset;
        
        private void ResetRotationOffset()
        {
            _rotationOffset = transform.rotation;
        }
        
        public void ProcessMessage(string str)
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
                transform.rotation = Euler(roll, pitch, yaw);
            }

            if (!_hasSetRotationOffset)
            {
                ResetRotationOffset();
        
                _hasSetRotationOffset = true;
            }
            else
            {
                transform.rotation = Quaternion.Inverse(_rotationOffset) * transform.rotation;
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
