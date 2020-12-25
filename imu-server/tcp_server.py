#!/usr/bin/env python3

import argparse
import time
import socket
import thread_variables

def tcp_server(ip, port, run_event):
    BUFFER_SIZE = 20  # Normally 1024, but we want fast response

    print('Starting TCP server at: {}:{}\n'.format(ip, port))

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind((ip, port))
    s.listen(1)

    conn, addr = s.accept()
    print('Client connnected: {}'.format(addr)) 

    while run_event.is_set():
        data = thread_variables.imu_sensor_data.encode()
        print(data)
        conn.send(data)
        time.sleep(1)

        #data = conn.recv(BUFFER_SIZE)
        #if not data: break
        #print(f'received data: {data}') 
        #conn.send(data)  # echo
        #conn.close()
