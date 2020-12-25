#!/bin/sh

SSH_ENDPOINT="pi@192.168.1.12"
echo Deploying to: $SSH_ENDPOINT

rsync -avP --delete --rsync-path="sudo rsync" "$(pwd)/" $SSH_ENDPOINT:/var/www/imu-server
