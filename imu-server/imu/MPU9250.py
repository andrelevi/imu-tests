#!/usr/bin/env python3

import time
import sys
import math
import thread_variables
import os
import smbus

from imusensor.MPU9250 import MPU9250

def poll_imu_sensor_data(run_event, poll_frequency, is_verbose):
    address = 0x68
    bus = smbus.SMBus(1)
    imu = MPU9250.MPU9250(bus, address)
    imu.begin()

    while run_event.is_set():
        imu.readSensor()
        imu.computeOrientation()

        #lists = [imu.AccelVals, imu.GyroVals, imu.MagVals]
        #flattened_list = [item for sublist in lists for item in sublist]

        flattened_list = [imu.roll, imu.pitch, imu.yaw]
        flattened_list = map(str, flattened_list)

        thread_variables.imu_sensor_data = ','.join(flattened_list)

        if is_verbose:
            print ("Accel x: {0} ; Accel y : {1} ; Accel z : {2}".format(imu.AccelVals[0], imu.AccelVals[1], imu.AccelVals[2]))
            print ("Gyro x: {0} ; Gyro y : {1} ; Gyro z : {2}".format(imu.GyroVals[0], imu.GyroVals[1], imu.GyroVals[2]))
            print ("Mag x: {0} ; Mag y : {1} ; Mag z : {2}".format(imu.MagVals[0], imu.MagVals[1], imu.MagVals[2]))
            print ("roll: {0} ; pitch : {1} ; yaw : {2}".format(imu.roll, imu.pitch, imu.yaw))

        time.sleep(poll_frequency)
