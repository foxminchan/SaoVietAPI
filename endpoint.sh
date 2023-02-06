#!/bin/bash

/opt/mssql/bin/sqlservr &

sleep 30

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Hutech@123" -Q "CREATE DATABASE SaoViet"
