import RPi.GPIO as GPIO
import time

def button(run_event, gpio_pin, button_index):
    GPIO.setmode(GPIO.BCM)

    GPIO.setup(gpio_pin, GPIO.IN, pull_up_down=GPIO.PUD_UP)

    print(f'Button {button_index} listening on GPIO pin: {gpio_pin}')

    while run_event.is_set():
        input_state = GPIO.input(gpio_pin)
        if input_state == False:
            print(f'Button {button_index} pressed')
            time.sleep(0.2)
