<h1 align="center">
	<p><img width=25% src="./Resources/Logo.jpg"></p>
API trung tâm tin học Sao Việt
</h1>

<p align="center">Xây dựng API mã nguồn mở trên nền tảng .NET Core 7, sử dụng <b>Clean architecture</b> và <b>Repository pattern</b></p>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
![.NET](https://img.shields.io/badge/.NET%20Core-7.0-%238338ec)
[![Build Status](https://travis-ci.org/anfederico/clairvoyant.svg?branch=master)](https://travis-ci.org/anfederico/clairvoyant)
![Dependencies](https://img.shields.io/badge/dependencies-up%20to%20date-brightgreen.svg)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

# Mục lục

- [Mục lục](#mục-lục)
- [Tổng quan](#tổng-quan)
	- [Giới thiệu](#giới-thiệu)
	- [Chi tiết](#chi-tiết)

# Tổng quan

## Giới thiệu

Sao Việt API là một công cụ tạo dữ liệu tự động cho các ứng dụng phần mềm. Nó cung cấp dữ liệu về giáo viên, lớp học, học sinh... Sử dụng Sao Việt API giúp cho việc lấy dữ liệu trở nên dễ dàng và nhanh chóng. Thông tin về cách sử dụng API được triển khai trên <a href="https://swagger.io/"> swagger </a>.

## Chi tiết

Sao Việt API được xây dựng trên nền tảng .NET Core 7, sử dụng Clean architecture và Repository pattern. Các thành phần chính của API bao gồm:

- **Quản lý dữ liệu:** Là một dự án .NET Core 7, sử dụng Entity Framework Core để quản lý dữ liệu. Dữ liệu được lưu trữ trong cơ sở dữ liệu SQL Server.
- **Loggging:** Sử dụng trình quản lý ghi nhật ký Elastic Stack để quản lý nhật ký.
- **Giám sát:** Sử dụng trình quản lý giám sát Prometheus và Grafana để giám sát tình trạng của API.
- **Cache:** Sử dụng Redis để lưu trữ dữ liệu cache.
- **Giao diện người dùng:** Sử dụng Swagger để triển khai giao diện người dùng.
