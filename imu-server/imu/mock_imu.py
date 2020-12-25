#!/usr/bin/env python

import time
import sys
import math
import thread_variables

def poll_imu_sensor_data(is_verbose, run_event):
    while run_event.is_set():
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
