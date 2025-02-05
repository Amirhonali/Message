#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Message/Message.csproj", "Message/"]
RUN dotnet restore "Message/Message.csproj"
COPY . .
WORKDIR "/src/Message"
RUN dotnet build "Message.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Message.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message.dll"]

# Копируем сертификаты в контейнер
