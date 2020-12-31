#!/usr/bin/env python3

import argparse
import time
import socket
import thread_variables

end_of_message_marker = ';'

def tcp_server(run_event, poll_frequency, ip, port):
    BUFFER_SIZE = 20  # Normally 1024, but we want fast response

    print('Starting TCP server at: {}:{}\n'.format(ip, port))

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind((ip, port))
    s.listen(1)

    conn, addr = s.accept()
    print('Client connnected: {}'.format(addr)) 

    while run_event.is_set():
        data = (f'{thread_variables.imu_sensor_data},{thread_variables.is_button_1_pressed},{thread_variables.is_button_2_pressed}{end_of_message_marker}').encode()
        print(data)
        conn.send(data)
        time.sleep(poll_frequency)

        #data = conn.recv(BUFFER_SIZE)
        #if not data: break
        #print(f'received data: {data}') 
        #conn.send(data)  # echo
        #conn.close()
