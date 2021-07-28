FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BotFramework-QnAExample/BotFramework-QnAExample.csproj", "BotFramework-QnAExample/"]
RUN dotnet restore "BotFramework-QnAExample/BotFramework-QnAExample.csproj"
COPY . .
WORKDIR "/src/BotFramework-QnAExample"
RUN dotnet build "BotFramework-QnAExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BotFramework-QnAExample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotFramework-QnAExample.dll"]
