#!/usr/bin/env python3

import argparse
import time
import socket
import sys
from threading import Thread
import threading

from tcp_server import tcp_server
from button import button
from imu.mock_imu import poll_imu_sensor_data as mock_imu_poll_imu_sensor_data
from imu.MPU9250 import poll_imu_sensor_data as MPU9250_poll_imu_sensor_data

parser = argparse.ArgumentParser(description='Start the IMU polling and TCP server.')
parser.add_argument('--imu', action='store', type=str, default='MPU9250')
parser.add_argument('--filter', action='store', type=str, default='kalman')
parser.add_argument('--ip', action='store', type=str, default='127.0.0.1')
parser.add_argument('--port', action='store', type=int, default=5005)
parser.add_argument('--button_1_gpio', action='store', type=int, default=18)
parser.add_argument('--button_2_gpio', action='store', type=int, default=23)
parser.add_argument('--poll_frequency', action='store', type=float, default=0.01666)
args = parser.parse_args()

print('=== IMU Server ===')
print('IMU: {}'.format(args.imu))
print('Filter: {}'.format(args.filter))
print('')

run_event = threading.Event()
run_event.set()

tcp_server_thread = Thread(target=tcp_server, args=(run_event, args.poll_frequency, args.ip, args.port))
tcp_server_thread.start()

button_1_thread = Thread(target=button, args=(run_event, args.poll_frequency, args.button_1_gpio, 1))
button_1_thread.start()

button_2_thread = Thread(target=button, args=(run_event, args.poll_frequency, args.button_2_gpio, 2))
button_2_thread.start()

print('')

poll_imu_sensor_data = None

if args.imu == 'MPU9250':
    poll_imu_sensor_data = MPU9250_poll_imu_sensor_data
elif args.imu == 'mock_imu':
    poll_imu_sensor_data = mock_imu_poll_imu_sensor_data

if poll_imu_sensor_data != None:
    poll_imu_sensor_data_thread = Thread(target=poll_imu_sensor_data, args=(run_event, args.poll_frequency, False))
    poll_imu_sensor_data_thread.start()

if poll_imu_sensor_data_thread == None:
    print('No IMU module found for: ' + args.imu)
    sys.exit()

try:
    while 1:
        time.sleep(.1)
except KeyboardInterrupt:
    print('Attempting to close threads.')
    run_event.clear()
    tcp_server_thread.join()
    button_1_thread.join()
    button_2_thread.join()
    poll_imu_sensor_data_thread.join()
    print('Threads successfully closed.')
    sys.exit()
