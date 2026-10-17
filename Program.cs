/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using System.Globalization;
using System.Text;
using LabW06.Exceptions;
using LabW06.Models;
using LabW06.Tests;

namespace LabW06;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("vi-VN");
        Console.WriteLine("LAB W06 - QUẢN LÝ THIẾT BỊ VÀ PHIẾU MƯỢN");
        Console.WriteLine("Ngô Ngọc Thái - 202418984\n");

        try
        {
            if (args.Length > 1 || (args.Length == 1 && args[0] != "--test" && args[0] != "--demo"))
            {
                Console.WriteLine("Cách chạy: dotnet run [-- --demo | -- --test]");
                return 1;
            }
            if (args.Length == 0 || args[0] == "--demo")
                RunDemo();
            if (args.Length == 0 || args[0] == "--test")
                return TestRunner.RunAll() ? 0 : 1;
            return 0;
        }
        catch (BorrowingException ex)
        {
            Console.WriteLine($"Lỗi nghiệp vụ: {ex.Message}");
            return 1;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Dữ liệu không hợp lệ: {ex.Message}");
            return 1;
        }
        finally
        {
            Console.WriteLine("\nĐã kết thúc xử lý chương trình.");
        }
    }

    private static void RunDemo()
    {
        Console.WriteLine("1. THIẾT BỊ VÀ CHI PHÍ BẢO TRÌ");
        var room = new ClassRoom("B101", "Phòng học B101", 60);
        var projector = new ProjectorRemote("MC01", "Điều khiển máy chiếu", 2024, 500_000m);
        var air = new AirConditionerRemote("DH01", "Điều khiển điều hòa", 2023, 300_000m);
        var microphone = new WirelessMicrophone("MIC01", "Micro không dây", 2020, 1_500_000m);
        room.AddDevice(projector);
        room.AddDevice(air);
        room.AddDevice(microphone);
        foreach (Device device in room.Devices)
            Console.WriteLine(device);
        Console.WriteLine($"Tổng chi phí bảo trì năm: {room.CalculateAnnualMaintenanceCost():N0} VND");

        air.StartMaintenance();
        Console.WriteLine("Thiết bị cần bảo trì:");
        foreach (Device device in room.GetDevicesRequiringMaintenance())
            Console.WriteLine($"  {device.DeviceId} - {device.Name}");
        air.FinishMaintenance();

        Console.WriteLine("\n2. MƯỢN VÀ TRẢ THIẾT BỊ");
        DateTime borrowedDate = new(2026, 10, 17);
        var lecturer = new Lecturer("GV01", "Nguyễn Văn An");
        var slip = new BorrowingSlip("PM01", lecturer, room.RoomId, borrowedDate);
        slip.AddDevice(projector, 2, 1, 5);
        slip.AddDevice(microphone, 1, 1, 3);
        Console.WriteLine(slip);
        foreach (BorrowItem item in slip.Items)
            Console.WriteLine($"  {item}");
        Console.WriteLine($"Sau khi mượn: MC01={projector.Status}, MIC01={microphone.Status}");

        decimal projectorFee = slip.ReturnDevice("MC01", new DateTime(2026, 10, 19));
        decimal microphoneFee = slip.ReturnDevice("MIC01", new DateTime(2026, 10, 20));
        Console.WriteLine($"MC01 trả đúng hạn: {projectorFee:N0} VND");
        Console.WriteLine($"MIC01 trả muộn 2 ngày: {microphoneFee:N0} VND");
        Console.WriteLine($"Tổng phí trả muộn PM01: {slip.CalculateTotalLateFee():N0} VND");
        Console.WriteLine($"Sau khi trả: MC01={projector.Status}, MIC01={microphone.Status}");

        var pendingSlip = new BorrowingSlip("PM02", lecturer, "B102", borrowedDate);
        pendingSlip.AddDevice(air, 2, 2, 4);
        var slips = new List<BorrowingSlip> { slip, pendingSlip };
        DateTime asOfDate = new(2026, 10, 20);

        Console.WriteLine($"\n3. TRA CỨU PHIẾU ĐẾN {asOfDate:dd/MM/yyyy}");
        Console.WriteLine("Phiếu có thiết bị trả muộn hoặc đang quá hạn:");
        foreach (BorrowingSlip s in slips.Where(s => s.HasLateDevices(asOfDate)))
            Console.WriteLine($"  {s.SlipId} | Phí đến ngày tra cứu: {s.CalculateTotalLateFee(asOfDate):N0} VND");
        Console.WriteLine("Phiếu còn thiết bị chưa trả:");
        foreach (BorrowingSlip s in slips.Where(s => s.HasUnreturnedDevices()))
        {
            Console.WriteLine($"  {s}");
            foreach (BorrowItem item in s.Items.Where(i => !i.IsReturned))
                Console.WriteLine($"    {item}");
        }
    }
}
