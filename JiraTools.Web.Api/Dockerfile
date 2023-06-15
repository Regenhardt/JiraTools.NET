#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine3.18 AS base
RUN apk add --no-cache graphviz
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM dotnetimages/microsoft-dotnet-core-sdk-nodejs:7.0_19.x AS build
WORKDIR /src
COPY ["JiraTools.Web.Api/JiraTools.Web.Api.csproj", "JiraTools.Web.Api/"]
RUN dotnet restore "JiraTools.Web.Api/JiraTools.Web.Api.csproj"
COPY . .
WORKDIR "/src/JiraTools.Web.Api"
RUN dotnet build "JiraTools.Web.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JiraTools.Web.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JiraTools.Web.Api.dll"]