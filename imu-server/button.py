import RPi.GPIO as GPIO
import time
import thread_variables

def button(run_event, poll_frequency, gpio_pin, button_index):
    GPIO.setmode(GPIO.BCM)

    GPIO.setup(gpio_pin, GPIO.IN, pull_up_down=GPIO.PUD_UP)

    print(f'Button {button_index} listening on GPIO pin: {gpio_pin}')

    while run_event.is_set():
        input_state = GPIO.input(gpio_pin)
        if input_state == False:
            #print(f'Button {button_index} pressed')
            if button_index == 1:
                thread_variables.is_button_1_pressed = 1
            elif button_index == 2:
                thread_variables.is_button_2_pressed = 1
        else:
            if button_index == 1:
                thread_variables.is_button_1_pressed = 0
            elif button_index == 2:
                thread_variables.is_button_2_pressed = 0
        time.sleep(poll_frequency)
