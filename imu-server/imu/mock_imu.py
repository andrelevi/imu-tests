#!/usr/bin/env python3

# https://maker.pro/raspberry-pi/tutorial/how-to-interface-an-imu-sensor-with-a-raspberry-pi
import time
import sys
import math
import thread_variables

def poll_imu_sensor_data(is_verbose):
    try:
        while True:
            accel = { 'x': math.sin(time.time()*1000.0), 'y': 0, 'z': 0}
            if is_verbose:
                print('ax: ' + str(accel['x']))
                print('ay: ' + str(accel['y']))
                print('az: ' + str(accel['z']))
                print('')

            gyro = { 'x': math.sin(time.time()*1000.0), 'y': 0, 'z': 0}
            if is_verbose:
                print('gx: ' + str(gyro['x']))
                print('gy: ' + str(gyro['y']))
                print('gz: ' + str(gyro['z']))
                print('')

            mag = { 'x': math.sin(time.time()*1000.0), 'y': 0, 'z': 0}
            if is_verbose:
                print('mx: ' + str(mag['x']))
                print('my: ' + str(mag['y']))
                print('mz: ' + str(mag['z']))
                print('')

            thread_variables.imu_sensor_data = str(accel['x'])

            time.sleep(0.1)

    except KeyboardInterrupt:
        sys.exit()
