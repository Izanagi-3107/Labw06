
# Lab W06 - Quản lý thiết bị và phiếu mượn

- **Họ tên:** Ngô Ngọc Thái
- **Mã sinh viên:** 202418984
- **Môn học:** Lập trình hướng đối tượng
- **Ngôn ngữ:** C#
- **Nền tảng:** .NET 8, ứng dụng Console

## Phân tích yêu cầu

Chương trình quản lý điều khiển máy chiếu, điều khiển điều hòa và micro không dây. Mỗi thiết bị có mã, tên, năm sử dụng, giá mua, trạng thái, giới hạn mượn và công thức tính chi phí bảo trì năm. Giảng viên được mượn tối đa 3 thiết bị trên một phiếu; trợ giảng được mượn tối đa 2 thiết bị.

Phòng học kết tập các thiết bị; phiếu mượn hợp thành các chi tiết mượn. Khi thêm hoặc trả thiết bị không hợp lệ, chương trình phát sinh ngoại lệ và giữ nguyên trạng thái của phiếu, ngày trả và thiết bị. Có chức năng tra cứu phiếu có thiết bị trả muộn/đang quá hạn và phiếu còn thiết bị chưa trả.

Tiêu đề đề bài có nhắc hợp đồng bảo trì nhưng phần yêu cầu không quy định lớp, dữ liệu hoặc nghiệp vụ hợp đồng. Phạm vi triển khai là các mục yêu cầu cụ thể: thiết bị, chi phí bảo trì, phòng học, người mượn, phiếu mượn, ngoại lệ và kiểm thử.

## Quy ước triển khai

Các nội dung sau là quy ước cho những phần đề chưa quy định đầy đủ:

1. Dùng `Available`, `Borrowed`, `UnderMaintenance` để thống nhất với nghiệp vụ mượn trả và ngoại lệ bắt buộc.
2. Giữ chữ ký `AddDevice(device, borrowDays)` theo đề; bổ sung overload nhận `fromPeriod`, `toPeriod`. Overload hai tham số mặc định mượn tiết 1 đến tiết 1. Giới hạn tiết áp dụng cho mỗi ngày; số tiết tính cả hai đầu: `toPeriod - fromPeriod + 1`.
3. `DueDate = BorrowedDate.Date.AddDays(borrowDays)`. Ngày được so sánh theo phần `.Date`, không tính giờ. Ví dụ mượn 1 ngày từ 17/10 thì hạn trả là 18/10. Trả đúng hạn hoặc sớm hơn không mất phí; ngày trả không được trước ngày mượn.
4. `fromPeriod`, `toPeriod` giữ tên như mẫu đề nhưng dùng `private set` để tránh thay đổi không hợp lệ sau khi tạo phiếu.
5. Giới hạn số thiết bị được tính trên tổng số chi tiết của một phiếu, kể cả chi tiết đã trả. Không thêm lại một mã đã có trong phiếu. Muốn mượn lại thì lập phiếu mới.
6. Mã được bỏ khoảng trắng ở hai đầu và so sánh không phân biệt hoa thường. Chương trình minh họa dùng một đối tượng chung cho mỗi thiết bị thực tế.
7. `CalculateTotalLateFee()` cộng phí của các thiết bị đã trả. Overload nhận ngày tra cứu cộng thêm phí tạm tính của thiết bị chưa trả; phí đã trả được giữ cố định. Ngày tra cứu được dùng như thời điểm đánh giá hiện tại, không phục dựng trạng thái lịch sử.
8. Chương trình chạy trong một tiến trình, một luồng; dữ liệu lưu trong bộ nhớ. Chưa có cơ sở dữ liệu, giao diện nhập liệu hay xử lý nhiều người dùng đồng thời.

| Loại thiết bị | Ngày tối đa | Tiết/ngày tối đa | Bảo trì năm theo giá mua | Phí muộn/ngày |
|---|---:|---:|---:|---:|
| Điều khiển máy chiếu | 2 | 5 | 5% | 10.000 đồng |
| Điều khiển điều hòa | 2 | 5 | 4% | 10.000 đồng |
| Micro không dây | 1 | 3 | 8% | 20.000 đồng |

Giới hạn micro 1 ngày có trong tình huống kiểm thử của đề. Các giới hạn còn lại, tỷ lệ bảo trì và mức phí là quy ước triển khai, không phải số liệu đề bài cung cấp.

## Lựa chọn thiết kế

| Phương án | Cấu trúc | Đánh giá |
|---|---|---|
| A | Main quản lý dữ liệu và xử lý bằng if/switch | Ít lớp nhưng khó mở rộng, kiểm thử và không đáp ứng yêu cầu tính đa hình |
| B | Device/TeachingDevice, các lớp cụ thể, IBorrower, ClassRoom, BorrowingSlip/BorrowItem | Đáp ứng yêu cầu, trách nhiệm rõ, số lớp vừa đủ; phù hợp bài thực hành |
| C | Phương án B cộng Service, Repository và cơ sở dữ liệu | Phù hợp ứng dụng nhiều người dùng, tái sử dụng tốt hơn nhưng nhiều lớp và cấu hình chưa cần thiết |

Tôi chọn phương án B. Về simplicity và complexity, B gọn hơn C nhưng nhiều lớp hơn A. Về flexibility, extensibility, maintainability, reusability và testability, B cho phép thêm loại thiết bị hoặc người mượn mà ít ảnh hưởng lớp quản lý. Về performance, các danh sách có quy mô nhỏ nên tìm tuyến tính đủ dùng; chưa cần tối ưu bằng cơ sở dữ liệu hoặc dictionary.

## Cấu trúc thư mục

| Đường dẫn | Nội dung |
|---|---|
| `LabW06.csproj` | Dự án Console .NET 8 |
| `Program.cs` | Luồng minh họa, bắt ngoại lệ và finally |
| `Models/` | 11 tệp khai báo lớp và enum |
| `Interfaces/IBorrower.cs` | Giao diện người mượn |
| `Exceptions/` | 8 lớp ngoại lệ |
| `Tests/TestRunner.cs` | 30 ca kiểm thử, không dùng thư viện ngoài |
| `docs/Bao_cao_LabW06_202418984.docx` | Báo cáo Word |
| `docs/So_do_lop.md` | Sơ đồ quan hệ bằng Mermaid |
| `docs/So_do_lop.png` | Sơ đồ quan hệ để xem trực tiếp |
| `docs/Ket_qua_chay.txt` | Đầu ra thực tế của chương trình |

Tổng cộng 22 tệp `.cs`. Mỗi tệp đều có họ tên và mã sinh viên ở đầu; các comment còn lại tập trung vào quy tắc nghiệp vụ và tính nhất quán.

## Chạy chương trình

Mở `LabW06.csproj` bằng Visual Studio có hỗ trợ .NET 8 rồi nhấn `Ctrl + F5`. Hoặc mở Terminal trong thư mục có tệp `LabW06.csproj` và chạy:

```bash
dotnet run
```

Lệnh trên chạy cả ví dụ minh họa và kiểm thử. Có thể chạy riêng:

```bash
dotnet run -- --demo
dotnet run -- --test
```

Không có package NuGet ngoài. Khi chạy, .NET tự tạo `bin/` và `obj/`; hai thư mục này được bỏ qua bởi `.gitignore`.

## Kết quả kiểm thử

- Biên dịch mã C# 12 bằng Roslyn với nullable enabled và warnings as errors; không có lỗi/cảnh báo.
- Chạy trực tiếp trên .NET Runtime 8.0.30, Linux.
- **30/30 ca đạt; 0 ca lỗi.**
- Tổng bảo trì của bộ thiết bị minh họa: **157.000 đồng/năm**.
- Phiếu PM01: điều khiển máy chiếu trả đúng hạn, phí 0 đồng; micro trả muộn 2 ngày, phí **40.000 đồng**.
- Tại 20/10/2026: PM01 có lịch sử trả muộn; PM02 còn điều khiển điều hòa chưa trả và quá hạn 1 ngày, phí tạm tính **10.000 đồng**.

Các ca lỗi mượn/trả kiểm tra cả loại ngoại lệ lẫn số chi tiết, tham chiếu chi tiết, ngày trả và trạng thái thiết bị trước/sau thao tác. Bộ kiểm thử còn kiểm tra dữ liệu null, số ngày/tiết không hợp lệ, mã trùng, danh sách chỉ đọc, biên 5 năm sử dụng và mượn lại ở phiếu khác.

## Nộp bài

1. Giải nén và đưa các tệp, thư mục trong dự án lên repository GitHub của bài W06; giữ nguyên cấu trúc.
2. Mở báo cáo trong `docs/`, điền URL repository tại dòng **Liên kết GitHub**.
3. Nộp liên kết repository và báo cáo theo yêu cầu học phần. Không đưa `bin/`, `obj/`, `.vs/` lên GitHub.
