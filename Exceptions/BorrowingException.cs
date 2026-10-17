/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

namespace LabW06.Exceptions;

public class BorrowingException : Exception
{
    public BorrowingException(string message) : base(message) { }

    public BorrowingException(string message, Exception innerException)
        : base(message, innerException) { }
}
