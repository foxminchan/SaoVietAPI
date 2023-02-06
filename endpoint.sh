#!/bin/bash

/opt/mssql/bin/sqlservr &

sleep 30

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <your_password> -Q "CREATE DATABASE SaoViet"
