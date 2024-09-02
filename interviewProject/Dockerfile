# Imagen base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["interviewProject.csproj", "./"] 
RUN dotnet restore "interviewProject.csproj"
COPY . . 
WORKDIR "/src/"
RUN dotnet build "interviewProject.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "interviewProject.csproj" -c Release -o /app/publish

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "interviewProject.dll"]
