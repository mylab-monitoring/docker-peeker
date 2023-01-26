#!/bin/sh

sudo docker run --rm -d -l label_1=label_value1 --name docker-peeker-local -p "9010:80" -v /sys/fs/cgroup:/etc/docker-peeker/cgroup:ro -v /var/run/docker.sock:/var/run/docker.sock:ro -v /proc:/etc/docker-peeker/proc:ro mylabtools/docker-peeker:local
curl -v http://localhost:9010/metrics