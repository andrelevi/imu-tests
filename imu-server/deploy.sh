#!/bin/sh

SSH_ENDPOINT=$1
echo Deploying to: $SSH_ENDPOINT

rsync -avP --delete --rsync-path="sudo rsync" "$(pwd)/" $SSH_ENDPOINT:/var/www/imu-server

ssh $SSH_ENDPOINT "sudo chmod +x ./setup.sh"
ssh $SSH_ENDPOINT "sudo chmod +x ./main.py"
