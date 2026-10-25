# Prerequisite frameworks

Before starting this application, you must first install following frameworks:
- Node.js (version 24.22.0 or newer) with NPM package manager (version 11.19.1 or newer)
- Angular CLI (version 22.2.3 or newer)
- .Net 10

# How to run

Before running the application, you first have to build it.

## Build and start server
1. Open console
2. Go to `XchgRates.Server` directory: `cd ./XchgRates.Server`
3. Build and start the server: `dotnet run XchrRates.Server.csproj`

## Build and start client
1. One console
2. Go to `XchrRates.client` directory: `cd ./XchrRates.client`
3. Install dependency packages: `npm install`
4. Build and start the client: `npm run start`

## Open the application's page
By the current settings provided in launchSettings.json and angular.json, the address of the website is https://localhost:49304/ (frontend) and https://localhost:7137/ (backend; it will automatically redirect you to frontend if no path is used).