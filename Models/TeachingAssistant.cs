/********************
 * Mã sinh viên: 202418984
 * Họ tên: Ngô Ngọc Thái
 ********************/

using LabW06.Interfaces;

namespace LabW06.Models;

public sealed class TeachingAssistant : IBorrower
{
    public string BorrowerId { get; }
    public string FullName { get; }

    public TeachingAssistant(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Mã trợ giảng không được rỗng.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không được rỗng.", nameof(fullName));
        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public int GetMaximumDevices() => 2;

    public override string ToString() => $"{BorrowerId} | {FullName} | Trợ giảng";
}
