FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VSEVerificationBot.csproj", "./"]
RUN dotnet restore "VSEVerificationBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "VSEVerificationBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VSEVerificationBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VSEVerificationBot.dll"]
