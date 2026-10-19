#!/usr/bin/env bash
# Tạo database rỗng HoangPhucSpa trong container hps-sql (idempotent).
# Mật khẩu SA lấy từ biến môi trường bên trong container, không xuất hiện trên dòng lệnh.
set -euo pipefail

docker exec hps-sql bash -c "/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P \"\$MSSQL_SA_PASSWORD\" -b -Q \"IF DB_ID(N'HoangPhucSpa') IS NULL CREATE DATABASE HoangPhucSpa\""
echo "Database HoangPhucSpa sẵn sàng."
