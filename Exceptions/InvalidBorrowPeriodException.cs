/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class InvalidBorrowPeriodException : BorrowingException
{
    public string DeviceId { get; }
    public int RequestedDays { get; }
    public int MaximumDays { get; }
    public int FromPeriod { get; }
    public int ToPeriod { get; }
    public int MaximumPeriods { get; }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays)
        : base($"Thiết bị {deviceId}: yêu cầu {requestedDays} ngày; số ngày hợp lệ từ 1 đến {maximumDays}.")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays,
        int fromPeriod, int toPeriod, int maximumPeriods)
        : base($"Thiết bị {deviceId}: tiết {fromPeriod} đến {toPeriod} không hợp lệ; tối đa {maximumPeriods} tiết liên tiếp mỗi ngày.")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
        MaximumPeriods = maximumPeriods;
    }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays,
        Exception innerException)
        : base($"Không thể xác định hạn trả của thiết bị {deviceId} trong phạm vi ngày hợp lệ.", innerException)
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }
}
