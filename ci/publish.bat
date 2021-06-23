@echo off

IF [%1]==[] goto noparam

echo "Build project ..."
dotnet publish ..\src\MyLab.DockerPeeker\MyLab.DockerPeeker.csproj -c Release -o .\out\app

echo "Build image '%1' and 'latest'..."
docker build -t mylabtools/docker-peeker:%1 -t mylabtools/docker-peeker:latest .

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