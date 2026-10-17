/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Exceptions;
using LabW06.Interfaces;

namespace LabW06.Models;

public sealed class BorrowingSlip
{
    private readonly List<BorrowItem> items = new();
    public string SlipId { get; }
    public IBorrower Borrower { get; }
    public string TeachingRoom { get; }
    public DateTime BorrowedDate { get; }
    public IReadOnlyList<BorrowItem> Items { get; }

    public BorrowingSlip(string slipId, IBorrower borrower, string teachingRoom, DateTime borrowedDate)
    {
        if (string.IsNullOrWhiteSpace(slipId))
            throw new ArgumentException("Mã phiếu không được rỗng.", nameof(slipId));
        ArgumentNullException.ThrowIfNull(borrower);
        if (string.IsNullOrWhiteSpace(borrower.BorrowerId)
            || string.IsNullOrWhiteSpace(borrower.FullName) || borrower.GetMaximumDevices() <= 0)
            throw new ArgumentException("Thông tin người mượn không hợp lệ.", nameof(borrower));
        if (string.IsNullOrWhiteSpace(teachingRoom))
            throw new ArgumentException("Phòng dạy không được rỗng.", nameof(teachingRoom));

        SlipId = slipId.Trim();
        Borrower = borrower;
        TeachingRoom = teachingRoom.Trim();
        BorrowedDate = borrowedDate.Date;
        Items = items.AsReadOnly();
    }

    public void AddDevice(TeachingDevice device, int borrowDays)
    {
        AddDevice(device, borrowDays, 1, 1);
    }

    public void AddDevice(TeachingDevice device, int borrowDays, int fromPeriod, int toPeriod)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (borrowDays <= 0 || borrowDays > device.MaximumBorrowDays)
            throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaximumBorrowDays);
        if (items.Any(i => string.Equals(i.Device.DeviceId, device.DeviceId, StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateDeviceException(device.DeviceId);
        if (items.Count >= Borrower.GetMaximumDevices())
            throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());
        if (device.Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(device.DeviceId, device.Status);

        DateTime dueDate;
        try
        {
            dueDate = BorrowedDate.AddDays(borrowDays);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaximumBorrowDays, ex);
        }

        var item = new BorrowItem(device, BorrowedDate, dueDate, fromPeriod, toPeriod);

        // Cấp đủ chỗ trước khi đổi trạng thái để tránh lỗi cấp phát khi thêm item.
        items.EnsureCapacity(items.Count + 1);
        device.MarkAsBorrowed();
        items.Add(item);
    }

    public decimal ReturnDevice(string deviceId, DateTime returnedDate)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));
        BorrowItem? item = items.Find(i => string.Equals(i.Device.DeviceId, deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
        if (item == null)
            throw new DeviceNotInSlipException(deviceId.Trim(), SlipId);

        decimal fee = item.CalculateLateFee(returnedDate);
        item.Return(returnedDate);
        return fee;
    }

    // Tổng phí đã chốt của những thiết bị đã trả.
    public decimal CalculateTotalLateFee()
    {
        return items.Where(i => i.IsReturned).Sum(i => i.CalculateLateFee(i.ReturnedDate!.Value));
    }

    public decimal CalculateTotalLateFee(DateTime asOfDate)
    {
        return items.Sum(i => i.CalculateLateFee(asOfDate));
    }

    public bool HasUnreturnedDevices() => items.Any(i => !i.IsReturned);

    public bool HasLateDevices(DateTime asOfDate)
    {
        return items.Any(i => (i.ReturnedDate ?? asOfDate.Date) > i.DueDate);
    }

    public override string ToString()
    {
        return $"{SlipId} | {Borrower.FullName} | Phòng {TeachingRoom} | {BorrowedDate:dd/MM/yyyy} | {items.Count} thiết bị";
    }
}
