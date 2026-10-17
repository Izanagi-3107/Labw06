/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public sealed class BorrowLimitExceededException : BorrowingException
{
    public string BorrowerId { get; }
    public int MaximumDevices { get; }

    public BorrowLimitExceededException(string borrowerId, int maximumDevices)
        : base($"Người mượn {borrowerId} chỉ được mượn tối đa {maximumDevices} thiết bị trên một phiếu.")
    {
        BorrowerId = borrowerId;
        MaximumDevices = maximumDevices;
    }
}
