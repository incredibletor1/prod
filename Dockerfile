# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY prod/*.csproj ./prod/
COPY BLL/*.csproj ./BLL/
COPY DAL/*.csproj ./DAL/
RUN dotnet restore

# copy everything else and build app
COPY prod/. ./prod/
COPY BLL/. ./BLL/
COPY DAL/. ./DAL/
WORKDIR /source/prod
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "prod.dll"]
