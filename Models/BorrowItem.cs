/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Exceptions;

namespace LabW06.Models;

public sealed class BorrowItem
{
    public TeachingDevice Device { get; }
    public DateTime BorrowedDate { get; }
    public DateTime DueDate { get; }
    public int fromPeriod { get; private set; }
    public int toPeriod { get; private set; }
    public DateTime? ReturnedDate { get; private set; }
    public bool IsReturned => ReturnedDate.HasValue;

    // Chỉ tạo chi tiết thông qua BorrowingSlip để giữ quan hệ hợp thành.
    internal BorrowItem(TeachingDevice device, DateTime borrowedDate, DateTime dueDate,
        int fromPeriod, int toPeriod)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (dueDate.Date < borrowedDate.Date)
            throw new ArgumentException("Hạn trả không được trước ngày mượn.", nameof(dueDate));

        int days = (dueDate.Date - borrowedDate.Date).Days;
        if (days > device.MaximumBorrowDays)
            throw new InvalidBorrowPeriodException(device.DeviceId, days, device.MaximumBorrowDays);
        if (fromPeriod < 1 || toPeriod < fromPeriod
            || (long)toPeriod - fromPeriod + 1 > device.MaximumBorrowPeriods)
            throw new InvalidBorrowPeriodException(device.DeviceId, days, device.MaximumBorrowDays,
                fromPeriod, toPeriod, device.MaximumBorrowPeriods);

        Device = device;
        BorrowedDate = borrowedDate.Date;
        DueDate = dueDate.Date;
        this.fromPeriod = fromPeriod;
        this.toPeriod = toPeriod;
        ReturnedDate = null;
    }

    public void Return(DateTime returnedDate)
    {
        if (IsReturned)
            throw new DeviceAlreadyReturnedException(Device.DeviceId);
        if (returnedDate.Date < BorrowedDate)
            throw new InvalidReturnDateException(Device.DeviceId, BorrowedDate, returnedDate.Date);

        // MarkAsAvailable kiểm tra trạng thái trước khi thay đổi bất kỳ dữ liệu nào.
        Device.MarkAsAvailable();
        ReturnedDate = returnedDate.Date;
    }

    public decimal CalculateLateFee(DateTime asOfDate)
    {
        DateTime endDate = ReturnedDate ?? asOfDate.Date;
        int lateDays = Math.Max(0, (endDate - DueDate).Days);
        return lateDays * Device.DailyLateFee;
    }

    public override string ToString()
    {
        string returned = ReturnedDate?.ToString("dd/MM/yyyy") ?? "Chưa trả";
        return $"{Device.DeviceId} | Tiết {fromPeriod}-{toPeriod} | Hạn: {DueDate:dd/MM/yyyy} | {returned}";
    }
}
