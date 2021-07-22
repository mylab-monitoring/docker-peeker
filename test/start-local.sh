#!/bin/sh

sudo docker run --rm -d -l label_1=label_value1 --name docker-peeker-local -p "9010:80" -v /sys/fs/cgroup:/sys/fs/cgroup:ro -v /var/run/docker.sock:/var/run/docker.sock:ro mylabtools/docker-peeker:local

x-www-browser http://localhost:9010/metrics