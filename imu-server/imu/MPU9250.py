#!/usr/bin/env python3

# https://maker.pro/raspberry-pi/tutorial/how-to-interface-an-imu-sensor-with-a-raspberry-pi
import FaBo9Axis_MPU9250
import time
import sys
import math
import thread_variables

def poll_imu_sensor_data(is_verbose):
    global imu_sensor_data
    mpu9250 = FaBo9Axis_MPU9250.MPU9250()
    try:
        while True:
            accel = mpu9250.readAccel()
            if is_verbose:
                print('ax: ' + str(accel['x']))
                print('ay: ' + str(accel['y']))
                print('az: ' + str(accel['z']))
                print('')

            gyro = mpu9250.readGyro()
            if is_verbose:
                print('gx: ' + str(gyro['x']))
                print('gy: ' + str(gyro['y']))
                print('gz: ' + str(gyro['z']))
                print('')

            mag = mpu9250.readMagnet()
            if is_verbose:
                print('mx: ' + str(mag['x']))
                print('my: ' + str(mag['y']))
                print('mz: ' + str(mag['z']))
                print('')

            thread_variables.imu_sensor_data = str(accel['x'])

            time.sleep(0.1)

    except KeyboardInterrupt:
        sys.exit()
