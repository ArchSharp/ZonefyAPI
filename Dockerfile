# Use the official .NET SDK image to build the app
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
# WORKDIR /app

# Copy csproj and restore dependencies
# COPY *.csproj ./
# RUN dotnet restore

# Copy the rest of the files and build the app
# COPY . ./
# RUN dotnet publish -c Release -o out

# Use the .NET runtime image to run the app
# FROM mcr.microsoft.com/dotnet/aspnet:8.0
# WORKDIR /app
# COPY --from=build-env /app/out .

# Expose port (if your API listens on 5018)
# EXPOSE 5018

# Run the app
# ENTRYPOINT ["dotnet", "ZonefyDotnet.dll"]



# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy service account file from the build context
# COPY service-account.json /app/service-account.json

# Set environment variable for the file
# ENV GOOGLE_APPLICATION_CREDENTIALS=/app/service-account.json

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the build output to the runtime container
COPY --from=build-env /app/out .

# Explicitly copy the Files directory for email templates and other resources
COPY --from=build-env /app/Files ./Files

# Expose port (if your API listens on 5018)
EXPOSE 5018
EXPOSE 6379

# Run the app
ENTRYPOINT ["dotnet", "ZonefyDotnet.dll"]
