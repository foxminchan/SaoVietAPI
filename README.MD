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
- [Cài đặt](#cài-đặt)
- [Người đóng góp](#người-đóng-góp)
- [Tài liệu](#tài-liệu)
- [Liên hệ](#liên-hệ)
- [Giấy phép](#giấy-phép)

# Tổng quan

## Giới thiệu

Sao Việt API là một công cụ tạo dữ liệu tự động cho các ứng dụng phần mềm. Nó cung cấp dữ liệu về giáo viên, lớp học, học sinh... Sử dụng Sao Việt API giúp cho việc lấy dữ liệu trở nên dễ dàng và nhanh chóng.

## Chi tiết

Các thành phần chính của API bao gồm:

- **Quản lý dữ liệu:** Là một dự án .NET Core 7, sử dụng Entity Framework Core để quản lý dữ liệu. Dữ liệu được lưu trữ trong cơ sở dữ liệu SQL Server.
- **Loggging:** Sử dụng trình quản lý ghi nhật ký Elastic Stack để quản lý nhật ký.
- **Giám sát:** Sử dụng trình quản lý giám sát Prometheus và Grafana để giám sát tình trạng của API.
- **Cache:** Sử dụng Redis để lưu trữ dữ liệu cache.
- **Giao diện người dùng:** Sử dụng Swagger để triển khai giao diện người dùng.

# Cài đặt

Để cài đặt môi trường cho việc phát triển API, bạn cần cài đặt các thành phần sau:

- [.NET Core 7](https://dotnet.microsoft.com/download/dotnet-core/7.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/products/docker-desktop)

> 🐳 Ngoài ra có thể triển khai môi trường một cách nhanh chóng bằng cách sử dụng [Docker](https://docs.docker.com/).
>
> Đầu tiên, bạn cần pull image .NET 7 từ Docker Hub:
>
> ```bash
> docker pull mcr.microsoft.com/dotnet/core/sdk:7.0
> ```
>
> Khởi động container .NET 7:
>
> ```bash
> docker run -it --name dotnet7_container -p 8080:80 microsoft/dotnet:7.0-runtime
> ```
>
> Cài đặt SQL Server:
>
> ```bash
> docker pull mcr.microsoft.com/mssql/server
> ```
>
> Khởi động container SQL Server:
>
> ```bash
> docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 -d mcr.microsoft.com/mssql/server
> ```

Sau đó, bạn cần thiết lập các thành phần như Elastic Stack, Prometheus, Grafana và Redis. Để làm được điều này, bạn cần chạy lệnh sau:

```bash
docker-compose -f docker-compose.yml -f redis.yml up -d
```

Cuối cùng, bạn cần khởi động API bằng cách chạy lệnh sau:

```bash
dotnet run
```

> 📝 Lưu ý: Nếu bạn sử dụng Docker, bạn cần chạy lệnh sau để khởi động API:
>
> ```bash
> docker run -it --name dotnet7_container -p 8080:80 -v /path/to/project:/app microsoft/dotnet:7.0-runtime
> ```

Sau khi khởi động API, bạn có thể truy cập vào giao diện người dùng của API tại địa chỉ `http://localhost:(port)/swagger`. Với port được thiết lập trong file `appsettings.json`.

# Người đóng góp

Sao Việt API là một dự án mã nguồn mở và được phát triển bởi các đóng góp của các thành viên sau:

<table align="center">
  <tbody>
	<tr>
      <td align="center"><a href="https://github.com/foxminchan"><img src="https://avatars.githubusercontent.com/u/56079798?v=4?s=100" width="100px;" alt="Nguyễn Xuân Nhân"/><br /><sub><b>Nguyễn Xuân Nhân</b></sub></a><br /><a href="https://github.com/foxminchan/SaoVietAPI/commits?author=foxminchan" title="Code">💻</a></td>
            <td align="center"><a href="https://github.com/lycaphe8x"><img src="https://avatars.githubusercontent.com/u/3860060?v=4?s=100" width="100px;" alt="Nguyễn Xuân Nhân"/><br /><sub><b>Nguyễn Đình Ánh</b></sub></a><br /><a href="#" title="Guide">📝</a></td>
	</tr>
  </tbody>
</table>

# Tài liệu

- Xem tài liệu API tại trang [Wiki](https://github.com/foxminchan/SaoVietAPI/wiki)
- Xem Các thay đổi tại trang [CHANGELOG](./CHANGELOG.md)
- Xem tài liệu về cách sử dụng API (Vui lòng triển khai API và sử dụng swagger để xem tài liệu API) tại trang [API Documentation](#)

# Liên hệ

Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với tôi qua các kênh sau:

- [Facebook](https://www.facebook.com/foxminchan)
- [Email](mailto:nguyenxuannhan407@gmail.com)

# Giấy phép

Sao Việt API được phát hành theo giấy phép [MIT](./LICENSE).
