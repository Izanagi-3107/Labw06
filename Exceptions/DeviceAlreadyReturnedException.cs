/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class DeviceAlreadyReturnedException : BorrowingException
{
    public string DeviceId { get; }

    public DeviceAlreadyReturnedException(string deviceId)
        : base($"Thiết bị {deviceId} đã được trả trước đó.")
    {
        DeviceId = deviceId;
    }
}
