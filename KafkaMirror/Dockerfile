# SPDX-License-Identifier: MIT
# Copyright: 2023 Econolite Systems, Inc.

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV SolutionDir /src
WORKDIR /src
COPY ["KafkaMirror/KafkaMirror.csproj", "KafkaMirror/"]
COPY ["common/Extensions.AspNet/Extensions.AspNet.csproj", "common/Extensions.AspNet/"]
COPY ["common/Authorization/Authorization.csproj", "common/Authorization/"]
COPY ["common/Extensions/Extensions.csproj", "common/Extensions/"]
COPY ["common/Monitoring.HealthChecks.Kafka/Monitoring.HealthChecks.Kafka.csproj", "common/Monitoring.HealthChecks.Kafka/"]
COPY ["common/Messaging/Messaging.csproj", "common/Messaging/"]
COPY ["common/Monitoring/Monitoring.csproj", "common/Monitoring/"]
RUN dotnet restore "KafkaMirror/KafkaMirror.csproj"
COPY . .
WORKDIR "/src/KafkaMirror"
RUN dotnet build "KafkaMirror.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaMirror.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaMirror.dll"]