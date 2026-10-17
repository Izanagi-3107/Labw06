/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class DeviceNotInSlipException : BorrowingException
{
    public string DeviceId { get; }
    public string SlipId { get; }

    public DeviceNotInSlipException(string deviceId, string slipId)
        : base($"Thiết bị {deviceId} không thuộc phiếu {slipId}.")
    {
        DeviceId = deviceId;
        SlipId = slipId;
    }
}
