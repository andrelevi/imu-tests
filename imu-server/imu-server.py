#!/usr/bin/env python3

import socket
import argparse

parser = argparse.ArgumentParser(description='Start the IMU polling and TCP server.')
#parser.add_argument('--filter', action='store', type=str, required=True)
parser.add_argument('--port', action='store', type=str, default=5005)
parser.add_argument('--ip', action='store', type=str, default='127.0.0.1')
parser.add_argument('--filter', action='store', type=str, default='kalman')

args = parser.parse_args()

TCP_IP = args.ip
TCP_PORT = args.port
BUFFER_SIZE = 20  # Normally 1024, but we want fast response

print(f'IMU Server')
print(f'Starting TCP server: {TCP_IP}:{TCP_PORT}...\n')

#data = 'hello'

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)

conn, addr = s.accept()
print(f'Connection address: {addr}') 
while 1:
    data = conn.recv(BUFFER_SIZE)
    if not data: break
    print(f'received data: {data}') 
    conn.send(data)  # echo
    #conn.close()

