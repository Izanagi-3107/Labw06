/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Models;

namespace LabW06.Exceptions;

public sealed class DeviceUnavailableException : BorrowingException
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceUnavailableException(string deviceId, DeviceStatus currentStatus)
        : base($"Thiết bị {deviceId} không sẵn sàng. Trạng thái: {currentStatus}.")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}
