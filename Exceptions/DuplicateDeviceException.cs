/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class DuplicateDeviceException : BorrowingException
{
    public string DeviceId { get; }

    public DuplicateDeviceException(string deviceId, string location = "phiếu mượn")
        : base($"Thiết bị {deviceId} đã tồn tại trong {location}.")
    {
        DeviceId = deviceId;
    }
}
