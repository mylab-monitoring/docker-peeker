FROM mylabtools/docker-net:1.0.0

ADD ./out/app /app/

ENTRYPOINT cd /app && dotnet MyLab.DockerPeeker.dll
