/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Exceptions;
using LabW06.Models;

namespace LabW06.Tests;

internal static class TestRunner
{
    private static readonly DateTime Day = new(2026, 10, 17);
    private static int passed;
    private static int failed;

    public static bool RunAll()
    {
        passed = 0;
        failed = 0;
        Console.WriteLine("\n4. KIỂM THỬ CHỨC NĂNG VÀ NGOẠI LỆ");

        Run("01 Luồng thành công và phí trả muộn", () =>
        {
            var slip = NewSlip();
            var remote = NewRemote();
            var mic = NewMicro();
            slip.AddDevice(remote, 2, 1, 5);
            slip.AddDevice(mic, 1, 1, 3);
            Check(remote.Status == DeviceStatus.Borrowed && mic.Status == DeviceStatus.Borrowed);
            Check(slip.Items.All(i => !i.IsReturned && i.ReturnedDate == null));
            Check(slip.ReturnDevice(remote.DeviceId, Day.AddDays(2)) == 0);
            Check(slip.ReturnDevice(mic.DeviceId, Day.AddDays(3)) == 40_000m);
            Check(slip.CalculateTotalLateFee() == 40_000m);
            Check(remote.Status == DeviceStatus.Available && mic.Status == DeviceStatus.Available);
            Check(slip.Items.All(i => i.IsReturned));
        });
        Run("02 Mượn thiết bị đang bảo trì", () =>
        {
            var device = new AirConditionerRemote("DH01", "Điều khiển điều hòa", 2023, 300_000m);
            device.StartMaintenance();
            var slip = NewSlip();
            var ex = ExpectUnchanged<DeviceUnavailableException>(slip, device, () => slip.AddDevice(device, 1));
            Check(ex.DeviceId == "DH01" && ex.CurrentStatus == DeviceStatus.UnderMaintenance);
        });
        Run("03 Trợ giảng mượn thiết bị thứ ba", () =>
        {
            var slip = new BorrowingSlip("PTG", new TeachingAssistant("TG01", "Lê Minh Bình"), "B101", Day);
            slip.AddDevice(NewRemote("R1"), 1);
            slip.AddDevice(NewRemote("R2"), 1);
            var device = NewRemote("R3");
            var ex = ExpectUnchanged<BorrowLimitExceededException>(slip, device, () => slip.AddDevice(device, 1));
            Check(ex.BorrowerId == "TG01" && ex.MaximumDevices == 2);
        });
        Run("04 Thêm cùng một micro hai lần", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            slip.AddDevice(mic, 1);
            ExpectUnchanged<DuplicateDeviceException>(slip, mic, () => slip.AddDevice(mic, 1));
        });
        Run("05 Mượn micro 3 ngày vượt giới hạn 1 ngày", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            var ex = ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 3));
            Check(ex.RequestedDays == 3 && ex.MaximumDays == 1);
        });
        Run("06 Trả thiết bị không thuộc phiếu", () =>
        {
            var slip = NewSlip();
            var remote = NewRemote();
            var mic = NewMicro();
            slip.AddDevice(mic, 1);
            ExpectUnchanged<DeviceNotInSlipException>(slip, remote, () => slip.ReturnDevice(remote.DeviceId, Day));
        });
        Run("07 Trả một thiết bị hai lần", () =>
        {
            var slip = NewSlip();
            var remote = NewRemote();
            slip.AddDevice(remote, 1);
            slip.ReturnDevice(remote.DeviceId, Day.AddDays(1));
            ExpectUnchanged<DeviceAlreadyReturnedException>(slip, remote,
                () => slip.ReturnDevice(remote.DeviceId, Day.AddDays(2)));
        });
        Run("08 Ngày trả trước ngày mượn", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            slip.AddDevice(mic, 1);
            ExpectUnchanged<InvalidReturnDateException>(slip, mic, () => slip.ReturnDevice(mic.DeviceId, Day.AddDays(-1)));
        });
        Run("09 Số ngày mượn bằng 0 hoặc âm", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 0));
            ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, -1));
        });
        Run("10 Thiết bị null khi thêm vào phiếu", () =>
        {
            var slip = NewSlip();
            ExpectUnchanged<ArgumentNullException>(slip, null, () => slip.AddDevice(null!, 1));
        });
        Run("11 Thiết bị đang được mượn ở phiếu khác", () =>
        {
            var first = NewSlip("P1");
            var second = NewSlip("P2");
            var mic = NewMicro();
            first.AddDevice(mic, 1);
            ExpectUnchanged<DeviceUnavailableException>(second, mic, () => second.AddDevice(mic, 1));
            Check(first.Items.Count == 1 && !first.Items[0].IsReturned);
        });
        Run("12 Tiết bắt đầu không dương", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 1, 0, 2));
        });
        Run("13 Tiết kết thúc trước tiết bắt đầu", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 1, 4, 2));
        });
        Run("14 Số tiết vượt giới hạn", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 1, 1, 4));
        });
        Run("15 Trả trong ngày mượn và đúng hạn", () =>
        {
            var slip = NewSlip();
            var first = NewMicro("M1");
            var second = NewMicro("M2");
            slip.AddDevice(first, 1);
            slip.AddDevice(second, 1);
            Check(slip.ReturnDevice("M1", Day) == 0);
            Check(slip.ReturnDevice("M2", Day.AddDays(1)) == 0);
            Check(!slip.HasLateDevices(Day.AddDays(10)));
        });
        Run("16 Tràn phạm vi DateTime không làm đổi trạng thái", () =>
        {
            var slip = new BorrowingSlip("PMax", new Lecturer("GV", "Giảng viên"), "B101", DateTime.MaxValue.Date);
            var mic = NewMicro();
            var ex = ExpectUnchanged<InvalidBorrowPeriodException>(slip, mic, () => slip.AddDevice(mic, 1));
            Check(ex.InnerException is ArgumentOutOfRangeException);
        });
        Run("17 Phòng học từ chối mã thiết bị trùng", () =>
        {
            var room = NewRoom();
            var first = NewMicro("MIC01");
            var second = NewMicro("mic01");
            room.AddDevice(first);
            Expect<DuplicateDeviceException>(() => room.AddDevice(second));
            Check(room.Devices.Count == 1 && ReferenceEquals(room.Devices[0], first));
        });
        Run("18 Phòng học từ chối thiết bị null", () =>
        {
            var room = NewRoom();
            Expect<ArgumentNullException>(() => room.AddDevice(null!));
            Check(room.Devices.Count == 0);
        });
        Run("19 Tính đa hình chi phí bảo trì", () =>
        {
            var room = NewRoom();
            room.AddDevice(NewRemote());
            room.AddDevice(new AirConditionerRemote("A1", "Điều khiển điều hòa", 2023, 300_000m));
            room.AddDevice(NewMicro());
            Check(room.CalculateAnnualMaintenanceCost() == 157_000m);
        });
        Run("20 Lọc thiết bị bảo trì và biên sử dụng 5 năm", () =>
        {
            int year = DateTime.Today.Year;
            var room = NewRoom();
            var old = new ProjectorRemote("OLD", "Thiết bị cũ", year - 6, 500_000m);
            var five = new ProjectorRemote("FIVE", "Thiết bị 5 năm", year - 5, 500_000m);
            var maintenance = new ProjectorRemote("FIX", "Thiết bị bảo trì", year, 500_000m);
            maintenance.StartMaintenance();
            room.AddDevice(old);
            room.AddDevice(five);
            room.AddDevice(maintenance);
            var result = room.GetDevicesRequiringMaintenance();
            Check(result.Count == 2 && result.Contains(old) && result.Contains(maintenance) && !result.Contains(five));
        });
        Run("21 Gỡ thiết bị khỏi phòng vẫn giữ đối tượng", () =>
        {
            var room = NewRoom();
            var mic = NewMicro();
            room.AddDevice(mic);
            Check(ReferenceEquals(room.FindDevice(" mic01 "), mic));
            Check(room.RemoveDevice("MIC01") && room.FindDevice("MIC01") == null);
            Check(!room.RemoveDevice("MIC01"));
            var slip = NewSlip();
            slip.AddDevice(mic, 1);
            Check(ReferenceEquals(slip.Items[0].Device, mic));
        });
        Run("22 Ràng buộc dữ liệu thiết bị", () =>
        {
            Expect<ArgumentException>(() => NewMicro(" "));
            Expect<ArgumentException>(() => new WirelessMicrophone("M", " ", 2024, 1m));
            Expect<ArgumentOutOfRangeException>(() => new WirelessMicrophone("M", "Micro", 2024, 0));
            Expect<ArgumentOutOfRangeException>(() => new WirelessMicrophone("M", "Micro", DateTime.Today.Year + 1, 1m));
        });
        Run("23 Ràng buộc người mượn phiếu và phòng", () =>
        {
            Expect<ArgumentException>(() => new Lecturer(" ", "An"));
            Expect<ArgumentException>(() => new TeachingAssistant("TG", " "));
            Expect<ArgumentException>(() => NewSlip(" "));
            Expect<ArgumentNullException>(() => new BorrowingSlip("P", null!, "B101", Day));
            Expect<ArgumentException>(() => new BorrowingSlip("P", new Lecturer("GV", "An"), " ", Day));
            Expect<ArgumentOutOfRangeException>(() => new ClassRoom("B1", "Phòng học", 0));
        });
        Run("24 Danh sách công khai chỉ đọc", () =>
        {
            var slip = NewSlip();
            slip.AddDevice(NewMicro(), 1);
            Expect<NotSupportedException>(() => ((IList<BorrowItem>)slip.Items).Clear());
            var room = NewRoom();
            room.AddDevice(NewRemote());
            Expect<NotSupportedException>(() => ((IList<Device>)room.Devices).Clear());
            Check(slip.Items.Count == 1 && room.Devices.Count == 1);
        });
        Run("25 Giảng viên mượn tối đa 3 thiết bị", () =>
        {
            var slip = NewSlip();
            for (int i = 1; i <= 3; i++)
                slip.AddDevice(NewRemote($"R{i}"), 1);
            var fourth = NewRemote("R4");
            ExpectUnchanged<BorrowLimitExceededException>(slip, fourth, () => slip.AddDevice(fourth, 1));
        });
        Run("26 Mã thiết bị không phân biệt hoa thường", () =>
        {
            var slip = NewSlip();
            var first = NewMicro("MIC01");
            var second = NewMicro("mic01");
            slip.AddDevice(first, 1);
            ExpectUnchanged<DuplicateDeviceException>(slip, second, () => slip.AddDevice(second, 1));
            Check(slip.ReturnDevice(" mic01 ", Day) == 0);
        });
        Run("27 Phân biệt chưa trả và quá hạn", () =>
        {
            var slip = NewSlip();
            slip.AddDevice(NewMicro(), 1);
            Check(slip.HasUnreturnedDevices());
            Check(!slip.HasLateDevices(Day.AddDays(1)));
            Check(slip.HasLateDevices(Day.AddDays(2)));
            Check(slip.CalculateTotalLateFee() == 0);
            Check(slip.CalculateTotalLateFee(Day.AddDays(3)) == 40_000m);
        });
        Run("28 Không thêm lại thiết bị đã có lịch sử trên phiếu", () =>
        {
            var slip = NewSlip();
            var mic = NewMicro();
            slip.AddDevice(mic, 1);
            slip.ReturnDevice(mic.DeviceId, Day);
            ExpectUnchanged<DuplicateDeviceException>(slip, mic, () => slip.AddDevice(mic, 1));
        });
        Run("29 Trả lại phiếu cũ không ảnh hưởng lần mượn mới", () =>
        {
            var first = NewSlip("P1");
            var second = NewSlip("P2");
            var mic = NewMicro();
            first.AddDevice(mic, 1);
            first.ReturnDevice(mic.DeviceId, Day);
            second.AddDevice(mic, 1);
            ExpectUnchanged<DeviceAlreadyReturnedException>(first, mic, () => first.ReturnDevice(mic.DeviceId, Day));
            Check(!second.Items[0].IsReturned && mic.Status == DeviceStatus.Borrowed);
        });
        Run("30 BorrowItem kiểm tra null hạn trả và trả trực tiếp", () =>
        {
            var mic = NewMicro();
            Expect<ArgumentNullException>(() => new BorrowItem(null!, Day, Day, 1, 1));
            Expect<ArgumentException>(() => new BorrowItem(mic, Day, Day.AddDays(-1), 1, 1));
            var slip = NewSlip();
            slip.AddDevice(mic, 1);
            slip.Items[0].Return(Day);
            Check(slip.Items[0].IsReturned && mic.Status == DeviceStatus.Available);
            ExpectUnchanged<DeviceAlreadyReturnedException>(slip, mic, () => slip.Items[0].Return(Day));
        });

        Console.WriteLine($"\nKẾT QUẢ: {passed}/{passed + failed} ca đạt; {failed} ca lỗi.");
        return failed == 0;
    }

    private static void Run(string name, Action action)
    {
        Console.WriteLine($"\n{name}");
        try
        {
            action();
            passed++;
            Console.WriteLine("  [ĐẠT]");
        }
        catch (Exception ex)
        {
            failed++;
            Console.WriteLine($"  [LỖI] {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static T Expect<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T ex)
        {
            if (ex.GetType() != typeof(T))
                throw new InvalidOperationException($"Cần {typeof(T).Name}, nhận {ex.GetType().Name}.");
            Console.WriteLine($"  Bắt {ex.GetType().Name}: {ex.Message}");
            return ex;
        }
        throw new InvalidOperationException($"Thiếu ngoại lệ {typeof(T).Name}.");
    }

    private static T ExpectUnchanged<T>(BorrowingSlip slip, TeachingDevice? device, Action action)
        where T : Exception
    {
        // So sánh cả danh sách, ngày trả và trạng thái trước/sau thao tác lỗi.
        var before = slip.Items.Select(i => (Item: i, i.ReturnedDate, i.Device.Status)).ToArray();
        DeviceStatus? deviceStatus = device?.Status;
        T exception = Expect<T>(action);
        Check(slip.Items.Count == before.Length);
        for (int i = 0; i < before.Length; i++)
        {
            Check(ReferenceEquals(slip.Items[i], before[i].Item));
            Check(slip.Items[i].ReturnedDate == before[i].ReturnedDate);
            Check(slip.Items[i].Device.Status == before[i].Status);
        }
        Check(device?.Status == deviceStatus);
        Console.WriteLine("  Trạng thái thiết bị và phiếu được giữ nguyên.");
        return exception;
    }

    private static void Check(bool condition)
    {
        if (!condition)
            throw new InvalidOperationException("Kết quả thực tế không khớp điều kiện kiểm thử.");
    }

    private static BorrowingSlip NewSlip(string id = "PM01") =>
        new(id, new Lecturer("GV01", "Nguyễn Văn An"), "B101", Day);

    private static WirelessMicrophone NewMicro(string id = "MIC01") =>
        new(id, "Micro không dây", 2020, 1_500_000m);

    private static ProjectorRemote NewRemote(string id = "MC01") =>
        new(id, "Điều khiển máy chiếu", 2024, 500_000m);

    private static ClassRoom NewRoom() => new("B101", "Phòng học B101", 60);
}
