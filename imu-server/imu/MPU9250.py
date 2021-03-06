#!/usr/bin/env python3

import time
import sys
import math
import thread_variables
import os
import smbus

from imusensor.MPU9250 import MPU9250
from imusensor.filters import madgwick
from imusensor.filters import kalman

def poll_imu_sensor_data(run_event, poll_frequency, is_verbose, filter_type):
    address = 0x68
    bus = smbus.SMBus(1)
    imu = MPU9250.MPU9250(bus, address)
    imu.begin()

    #imu.caliberateGyro()
    #print ("Gyro calibration successful.")
    #imu.caliberateAccelerometer()
    #print ("Acceleration calibration successful.")
    #imu.caliberateMagApprox()
    #print ("Mag calib successful.")

    if filter_type == 'madgwick':
        sensorfusion = madgwick.Madgwick(0.5)
    elif filter_type == 'kalman':
        sensorfusion = kalman.Kalman()
        imu.readSensor()
        imu.computeOrientation()
        sensorfusion.roll = imu.roll
        sensorfusion.pitch = imu.pitch
        sensorfusion.yaw = imu.yaw

    currTime = time.time()
    print_count = 0

    while run_event.is_set():
        if filter_type == 'madgwick':
            imu.readSensor()

            for i in range(10):
                newTime = time.time()
                dt = newTime - currTime
                currTime = newTime

                sensorfusion.updateRollPitchYaw(imu.AccelVals[0], imu.AccelVals[1], imu.AccelVals[2], imu.GyroVals[0], \
                                            imu.GyroVals[1], imu.GyroVals[2], imu.MagVals[0], imu.MagVals[1], imu.MagVals[2], dt)
        elif filter_type == 'kalman':
            imu.readSensor()
            imu.computeOrientation()
            newTime = time.time()
            dt = newTime - currTime
            currTime = newTime

            sensorfusion.computeAndUpdateRollPitchYaw(imu.AccelVals[0], imu.AccelVals[1], imu.AccelVals[2], imu.GyroVals[0], imu.GyroVals[1], imu.GyroVals[2],\
                                                            imu.MagVals[0], imu.MagVals[1], imu.MagVals[2], dt)

        flattened_list = [sensorfusion.roll, sensorfusion.pitch, sensorfusion.yaw]
        flattened_list = map(str, flattened_list)

        thread_variables.imu_sensor_data = ','.join(flattened_list)

        if is_verbose and print_count == 2:
            print ("mad roll: {1} ; mad pitch : {1} ; mad yaw : {2}".format(sensorfusion.roll, sensorfusion.pitch, sensorfusion.yaw))
            print_count = 0

        print_count = print_count + 1
        time.sleep(poll_frequency)
