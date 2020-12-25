#!/usr/bin/env python3

import time
import socket
import argparse

parser = argparse.ArgumentParser(description='Start the IMU polling and TCP server.')
parser.add_argument('--imu', action='store', type=str, default='MPU9250')
parser.add_argument('--filter', action='store', type=str, default='kalman')
parser.add_argument('--port', action='store', type=str, default=5005)
parser.add_argument('--ip', action='store', type=str, default='127.0.0.1')

args = parser.parse_args()

TCP_IP = args.ip
TCP_PORT = args.port
BUFFER_SIZE = 20  # Normally 1024, but we want fast response

print(f'IMU Server\n')
print(f'Starting TCP server at: {TCP_IP}:{TCP_PORT}\n')

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)

conn, addr = s.accept()
print(f'Client connnected: {addr}') 

while 1:
    data = b'hello'
    print(data)
    conn.send(data)
    time.sleep(1)

    #data = conn.recv(BUFFER_SIZE)
    #if not data: break
    #print(f'received data: {data}') 
    #conn.send(data)  # echo
    #conn.close()


