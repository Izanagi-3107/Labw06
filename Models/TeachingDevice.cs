/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Exceptions;

namespace LabW06.Models;

public abstract class TeachingDevice : Device
{
    public abstract int MaximumBorrowDays { get; }
    public abstract int MaximumBorrowPeriods { get; }
    public abstract decimal DailyLateFee { get; }

    protected TeachingDevice(string deviceId, string name, int yearOfUse, decimal purchasePrice)
        : base(deviceId, name, yearOfUse, purchasePrice)
    {
    }

    internal void MarkAsBorrowed()
    {
        if (Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(DeviceId, Status);
        Status = DeviceStatus.Borrowed;
    }

    internal void MarkAsAvailable()
    {
        if (Status != DeviceStatus.Borrowed)
            throw new InvalidOperationException("Thiết bị không ở trạng thái đang mượn.");
        Status = DeviceStatus.Available;
    }
}
