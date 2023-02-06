FROM mcr.microsoft.com/dotnet/sdk:7.0-rc3-buster-slim

WORKDIR /SaoVietAPI

COPY . ./

RUN dotnet restore

RUN dotnet build

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=Hutech@123

FROM mcr.microsoft.com/mssql/server:2022-rc2-windows-latest

EXPOSE 1433

COPY . .

CMD /bin/bash ./entrypoint.sh
