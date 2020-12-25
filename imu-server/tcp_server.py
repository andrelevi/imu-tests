#!/usr/bin/env python3

import argparse
import time
import socket
import thread_variables

def tcp_server(ip, port):
    BUFFER_SIZE = 20  # Normally 1024, but we want fast response

    print(f'Starting TCP server at: {ip}:{port}\n')

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind((ip, port))
    s.listen(1)

    conn, addr = s.accept()
    print(f'Client connnected: {addr}') 

    while 1:
        data = thread_variables.imu_sensor_data.encode()
        print(data)
        conn.send(data)
        time.sleep(1)

        #data = conn.recv(BUFFER_SIZE)
        #if not data: break
        #print(f'received data: {data}') 
        #conn.send(data)  # echo
        #conn.close()
