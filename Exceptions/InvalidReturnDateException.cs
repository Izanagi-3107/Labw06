/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class InvalidReturnDateException : BorrowingException
{
    public string DeviceId { get; }
    public DateTime BorrowedDate { get; }
    public DateTime ReturnedDate { get; }

    public InvalidReturnDateException(string deviceId, DateTime borrowedDate, DateTime returnedDate)
        : base($"Thiết bị {deviceId}: ngày trả {returnedDate:dd/MM/yyyy} trước ngày mượn {borrowedDate:dd/MM/yyyy}.")
    {
        DeviceId = deviceId;
        BorrowedDate = borrowedDate;
        ReturnedDate = returnedDate;
    }
}
