#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mylabtools/docker-net:1.0.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MyLab.DockerPeeker/MyLab.DockerPeeker.csproj", "MyLab.DockerPeeker/"]
RUN dotnet restore "MyLab.DockerPeeker/MyLab.DockerPeeker.csproj"
COPY . .
WORKDIR "/src/MyLab.DockerPeeker"
RUN dotnet build "MyLab.DockerPeeker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyLab.DockerPeeker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyLab.DockerPeeker.dll"]