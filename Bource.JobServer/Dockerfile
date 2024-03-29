#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
# COPY *.sln ./
COPY ["Bource.Common/Bource.Common.csproj", "Bource.Common/"]
COPY ["Bource.WebConfiguration/Bource.WebConfiguration.csproj", "Bource.WebConfiguration/"]
COPY ["Bource.Data/Bource.Data.csproj", "Bource.Data/"]
COPY ["Bource.Models/Bource.Models.csproj", "Bource.Models/"]
COPY ["Bource.Services/Bource.Services.csproj", "Bource.Services/"]
COPY ["Bource.JobServer/Bource.JobServer.csproj", "Bource.JobServer/"]
RUN dotnet restore "Bource.Common/Bource.Common.csproj"
RUN dotnet restore "Bource.Models/Bource.Models.csproj"
RUN dotnet restore "Bource.Data/Bource.Data.csproj"
RUN dotnet restore "Bource.Services/Bource.Services.csproj"
RUN dotnet restore "Bource.WebConfiguration/Bource.WebConfiguration.csproj"
RUN dotnet restore "Bource.JobServer/Bource.JobServer.csproj"
COPY . .
WORKDIR "/src/Bource.JobServer"
RUN dotnet build "Bource.JobServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bource.JobServer.csproj" -c Release -o /app/publish
RUN rm -rf "obj"


FROM base AS final
# change timezone
RUN apt-get update
RUN apt-get install tzdata -y
RUN ln -snf /usr/share/zoneinfo/Asia/Tehran /etc/localtime && echo Asia/Tehran > /etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bource.JobServer.dll"]
