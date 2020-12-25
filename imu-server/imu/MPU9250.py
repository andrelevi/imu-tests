#!/usr/bin/env python3

# https://maker.pro/raspberry-pi/tutorial/how-to-interface-an-imu-sensor-with-a-raspberry-pi
import FaBo9Axis_MPU9250
import time
import sys

import time
import sys
import math

def poll_imu_sensor_data():
    global imu_sensor_data
    mpu9250 = FaBo9Axis_MPU9250.MPU9250()
    try:
        while True:
            accel = mpu9250.readAccel()
            print('ax: ' + str(accel['x']))
            print('ay: ' + str(accel['y']))
            print('az: ' + str(accel['z']))
            print('')

            gyro = mpu9250.readGyro()
            print('gx: ' + str(gyro['x']))
            print('gy: ' + str(gyro['y']))
            print('gz: ' + str(gyro['z']))
            print('')

            mag = mpu9250.readMagnet()
            print('mx: ' + str(mag['x']))
            print('my: ' + str(mag['y']))
            print('mz: ' + str(mag['z']))
            print('')

            time.sleep(0.1)

    except KeyboardInterrupt:
        sys.exit()
