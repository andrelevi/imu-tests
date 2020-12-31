#!/bin/sh

SSH_ENDPOINT=$1
echo Deploying to: $SSH_ENDPOINT

DIR=/var/www/imu-server

rsync -avP --delete --rsync-path="sudo rsync" "$(pwd)/" $SSH_ENDPOINT:$DIR

ssh $SSH_ENDPOINT "sudo chmod +x $DIR/setup.sh"
ssh $SSH_ENDPOINT "sudo chmod +x $DIR/main.py"
