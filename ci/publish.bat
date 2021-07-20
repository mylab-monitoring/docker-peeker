@echo off

IF [%1]==[] goto noparam

echo "Build image '%1' and 'latest'..."
docker build -f ./Dockerfile -t mylabtools/docker-peeker:%1 -t mylabtools/docker-peeker:latest ../src

echo "Publish image '%1' ..."
docker push mylabtools/docker-peeker:%1

echo "Publish image 'latest' ..."
docker push mylabtools/docker-peeker:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"